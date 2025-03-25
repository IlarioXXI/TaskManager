using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.DataAccess;
using TaskManager.DataAccess.Repositories.Interfaces;
using TaskManager.DataAccess.Repositories;
using Microsoft.AspNetCore.Identity.UI.Services;
using TaskManager.DataAccess.Utility;
using System;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Obbliga HTTPS
    options.Cookie.SameSite = SameSiteMode.None; // Permette richieste cross-origin
    options.Cookie.IsEssential = true; // Obbliga il cookie a essere usato sempre

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
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
    //anche se l'utente non accetta cookie non essenziali la sessione funziona
    options.Cookie.IsEssential = true;
});


builder.Services.AddDistributedMemoryCache();
builder.Services.AddRazorPages();

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
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
        ctx.Context.Response.Headers.Append("Pragma", "no-cache");
        ctx.Context.Response.Headers.Append("Expires", "0");
    }
});
app.Use(async (context, next) =>
{
    Console.WriteLine("==== Nuova richiesta ====");
    Console.WriteLine($"URL: {context.Request.Path}");

    // Log cookie inviati dal client
    foreach (var cookie in context.Request.Cookies)
    {
        Console.WriteLine($"[Request] Cookie: {cookie.Key} = {cookie.Value}");
    }

    await next();

    // Log cookie inviati dal server
    if (context.Response.Headers.ContainsKey("Set-Cookie"))
    {
        Console.WriteLine($"[Response] Set-Cookie: {context.Response.Headers["Set-Cookie"]}");
    }
});
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=user}/{controller=Home}/{action=Index}/{id?}");

app.Run();
