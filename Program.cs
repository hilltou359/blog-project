using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using WeBlog.Configuration;
using WeBlog.Data;
using WeBlog.Models;
using WeBlog.Services;
using WeBlog.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

// Register PostgreSQL db connection

IConnectionService connectionService = new DefaultConnectionService();

DefaultConnectionService connectionS = new DefaultConnectionService();

//builder.Services.AddDbContext<ApplicationDbContext>(options =>
// We can use options.UseSqlServer according to the vid as well
// options.UseNpgsql(connectionService.GetConnectionString(builder.Configuration)));



//THIS IS WHAT IS BEING TESTED FOR THE ONLINE SERVICE FOR HEROKU
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//options.UseNpgsql(connectionService.GetConnectionString("BlogDb")));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseNpgsql(connectionService.GetConnectionString(builder.Configuration)));





/*THIS IS THE WORKING ONE THAT ALLOWED ME TO SUCCESSFULLY COMPILE THIS CODE, 
COMMENTED OUT TO TEST ONLINE SERVICE
builder.Services.AddDbContext<ApplicationDbContext>(options =>
 options.UseNpgsql(builder.Configuration.GetConnectionString("BlogDb")));
*/

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Register Identity class for authentication
builder.Services.AddIdentity<BlogUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddDefaultUI()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Add services to the container.

// Register DataService 
builder.Services.AddScoped<DataService>();

// Register BlogSearchService
builder.Services.AddScoped<BlogSearchService>();

// Register pre-configured instance of MailSettings class
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

// Register EmailService
builder.Services.AddScoped<IBlogEmailSender, EmailService>();

// Register ImageService
builder.Services.AddScoped<IImageService, DefaultImageService>();

// Register SlugService
builder.Services.AddScoped<ISlugService, DefaultSlugService>();

var app = builder.Build();

var dataService = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataService>();
await dataService.ManageDataAsync();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseMigrationsEndPoint();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
