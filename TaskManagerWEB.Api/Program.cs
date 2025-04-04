using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Text;
using TaskManager.DataAccess;
using TaskManager.DataAccess.Entities;
using TaskManager.DataAccess.Repositories;
using TaskManager.DataAccess.Repositories.Interfaces;
using TaskManagerWeb.Models;
using TaskManagerWEB.Api.models;

var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        options.EnableSensitiveDataLogging();
    });
        
       
// Add services to the container.
builder.Services.Configure<AppJWTSettings>(builder.Configuration.GetSection("JWTSettings"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    string issuer = builder.Configuration["JwtSettings:Issuer"]!;
    string audience = builder.Configuration["JwtSettings:Audience"]!;
    string secretKey = builder.Configuration["JwtSettings:SecretKey"]!;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();



builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddControllers();
builder.Services.AddMvc();



builder.Services.AddScoped<IValidator<ToDoVM>, TaskItemValidation>();
builder.Services.AddScoped<IValidator<TeamVM>, TeamValidation>();
builder.Services.AddScoped<IValidator<Comment>, CommentValidation>();
builder.Services.AddScoped<IValidator<RegisterModel>, RegisterValidation>();
builder.Services.AddScoped<IValidator<AuthUser>, AuthUserValidation>();
builder.Services.AddScoped<IValidator<ChangePasswordModel>, ChangePasswordValidation>();




// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Log.Logger = new LoggerConfiguration()             // Create a new Serilog logger configuration
                .ReadFrom.Configuration(builder.Configuration) // Read settings from appsettings.json
                .WriteTo.Console()                             // Log output to the console
                .WriteTo.File("logs/MyAppLog.txt")             // Log output to a file
                .CreateLogger();                               // Build the logger
                                                               // Replace the default logging provider with Serilog
builder.Host.UseSerilog(); // This ensures Serilog handles all logging



var app = builder.Build();


await SeedRoles(app.Services.CreateScope().ServiceProvider);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();

async Task SeedRoles(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roleNames = { "Admin", "User" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}

