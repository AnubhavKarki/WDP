using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WDP2024Assignment2.Data;
using WDP2024Assignment2.Models;

namespace WDP2024Assignment2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context; // Use the same DbContext

        public HomeController(ApplicationDbContext context)
        {
            _context = context; // Initialize your DbContext
        }

        public async Task<IActionResult> Index()
        {
            var images = await _context.AIImage.OrderByDescending(i => i.Like).ToListAsync(); // Fetch images ordered by likes
            return View(images); // Pass the images to the view
        }

        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
