using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using TaskManager.DataAccess;
using TaskManager.DataAccess.DBInitializer;
using TaskManager.DataAccess.Repositories;
using TaskManager.DataAccess.Repositories.Interfaces;
using TaskManager.DataAccess.Utility;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});


builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Add services to the container.
builder.Services.AddControllersWithViews();



builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    //se l'utente non fa nulla per 100 minuti la sessione scade
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    //HttpOnly è un flag che indica che il cookie non può essere letto da javascript quindi è più sicuro
    options.Cookie.HttpOnly = true;
    //anche se l'utente non accetta cookie non essenziali la sessione funziona
    options.Cookie.IsEssential = true;
});


builder.Services.AddDistributedMemoryCache();
builder.Services.AddRazorPages();

builder.Services.AddScoped<IDbInitializer,DbInitializer>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseHsts();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

SeedDatabase();

app.UseSession();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=user}/{controller=Home}/{action=Index}/{id?}");
//app.MapHub<DropDownHub>("/home");
app.Run();



void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize();
    }
}
