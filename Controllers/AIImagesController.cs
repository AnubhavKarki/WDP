using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WDP2024Assignment2.Data;
using WDP2024Assignment2.Models;

namespace WDP2024Assignment2.Controllers
{
    public class AIImagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AIImagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AIImages
        public async Task<IActionResult> Index()
        {
            // Fetch and return AIImages ordered by Like count in descending order
            var images = await _context.AIImage.OrderByDescending(i => i.Like).ToListAsync();
            return View(images);
        }

        // POST: AIImages/Like/5
        [HttpPost]
        public async Task<IActionResult> Like(int id)
        {
            var aIImage = await _context.AIImage.FindAsync(id);
            var userId = User.Identity.Name; // Get the current user's ID

            if (aIImage != null)
            {
                // Check if the user has already liked this image
                var userLikeExists = await _context.UserLikes
                    .AnyAsync(ul => ul.AIImageId == id && ul.UserId == userId);

                if (!userLikeExists)
                {
                    aIImage.Like++; // Increase the Like count
                    _context.UserLikes.Add(new UserLike { AIImageId = id, UserId = userId }); // Add a new like record
                    await _context.SaveChangesAsync(); // Save changes to the database
                }
            }

            return RedirectToAction(nameof(Index)); // Redirect back to the index page
        }

        // GET: AIImages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aIImage = await _context.AIImage.FirstOrDefaultAsync(m => m.Id == id);
            if (aIImage == null)
            {
                return NotFound();
            }

            return View(aIImage);
        }

        // GET: AIImages/Create
        public IActionResult Create()
        {
            var newAIImage = new AIImage
            {
                Like = 0 // Set default value for Like
            };
            return View(newAIImage);
        }

        // POST: AIImages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Prompt,ImageGenerator,UploadDate,Like,canIncreaseLike")] AIImage aIImage, IFormFile Filename)
        {
            if (ModelState.IsValid)
            {
                // Set default Like value if it hasn't been modified
                if (aIImage.Like == null)
                {
                    aIImage.Like = 0;
                }

                // Handle file upload
                if (Filename != null && Filename.Length > 0)
                {
                    // Set the upload date to the current date
                    aIImage.UploadDate = DateTime.Now;

                    // Generate a unique filename
                    var fileName = Path.GetFileName(Filename.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Filename.CopyToAsync(stream);
                    }

                    aIImage.Filename = fileName; // Store the filename in the model
                }

                // Add the AIImage to the context and save changes
                _context.Add(aIImage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(aIImage);
        }

        // GET: AIImages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aIImage = await _context.AIImage.FindAsync(id);
            if (aIImage == null)
            {
                return NotFound();
            }
            return View(aIImage);
        }

        // POST: AIImages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Prompt,ImageGenerator,UploadDate,Filename,Like,canIncreaseLike")] AIImage aIImage, IFormFile Filename)
        {
            if (id != aIImage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle file upload
                    if (Filename != null && Filename.Length > 0)
                    {
                        // Generate a unique filename
                        var fileName = Path.GetFileName(Filename.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                        // Optionally delete the old file
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", aIImage.Filename);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await Filename.CopyToAsync(stream);
                        }

                        aIImage.Filename = fileName; // Update the filename in the model
                    }

                    // Update the upload date if necessary
                    aIImage.UploadDate = DateTime.Now;
                    _context.Update(aIImage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AIImageExists(aIImage.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(aIImage);
        }

        // GET: AIImages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aIImage = await _context.AIImage.FirstOrDefaultAsync(m => m.Id == id);
            if (aIImage == null)
            {
                return NotFound();
            }

            return View(aIImage);
        }

        // POST: AIImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aIImage = await _context.AIImage.FindAsync(id);
            _context.AIImage.Remove(aIImage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AIImageExists(int id)
        {
            return _context.AIImage.Any(e => e.Id == id);
        }
    }
}
