using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WDP2024Assignment2.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Keep only the AddIdentity registration
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();

// Update database
builder.Services.BuildServiceProvider()
    .GetRequiredService<ApplicationDbContext>()
    .Database.Migrate();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();  // Ensure authentication middleware is added
app.UseAuthorization();

// Initialize roles and users
await InitializeRolesAndUsers(app.Services);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

// Method to initialize roles and users
async Task InitializeRolesAndUsers(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Create the "Member" role if it doesn't exist
    if (!await roleManager.RoleExistsAsync("Member"))
    {
        await roleManager.CreateAsync(new IdentityRole("Member"));
    }

    // Assign the "Member" role to the user
    var user = await userManager.FindByEmailAsync("member@asp.net");
    if (user != null && !await userManager.IsInRoleAsync(user, "Member"))
    {
        await userManager.AddToRoleAsync(user, "Member");
    }
}
