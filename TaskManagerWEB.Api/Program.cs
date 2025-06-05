using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using TaskManager.DataAccess;
using TaskManager.DataAccess.Interfaces;
using TaskManager.DataAccess.Repositories;
using TaskManager.Models;
using TaskManager.Services.Interfaces;
using TaskManager.Services.Middlewares;
using TaskManagerWeb.Api.ViewModels;
using TaskManagerWEB.Api.Validators;
using TaskManagerWEB.Api.Validators.UserValidators;
using TaskManagerWEB.Api.ViewModels.UserViewModels;
using TaskManager.Services.Services;
using TaskManagerWEB.Api.Configuration;
using TaskManagerWeb.Models;
using TaskManagerWeb.Api.Models;
using TaskManager.DataAccess.Utility;
using TaskManager.Services.ServicesInterfaces;
using TaskManagerWEB.Api.ViewModels;
using TeamVM = TaskManagerWEB.Api.ViewModels.TeamVM;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using TaskManagerWeb.Api.ViewModels.UserViewModels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CORSPolicy",
        builder =>
        {
            builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithOrigins("http://localhost:51678", "https://calm-water-04859b403.azurestaticapps.net");
        });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        options.EnableSensitiveDataLogging();
    });



builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
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
builder.Services.AddHttpContextAccessor();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});


builder.Services.AddAutoMapper(typeof(AutoMapperConfiguration));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserClaimService, UserClaimService>();
builder.Services.AddScoped<IApiIdentityService, ApiIdentityService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IHistoryService, HistoryService>();
builder.Services.AddScoped<ITaskItemService, TaskItemService>();
builder.Services.AddScoped<ITeamService, TeamService>();

builder.Services.AddControllers();
builder.Services.AddMvc();



builder.Services.AddScoped<IValidator<TaskItemVM>, TaskItemValidation > ();
builder.Services.AddScoped<IValidator<TeamVM>, TeamValidation>();
builder.Services.AddScoped<IValidator<CommentVM>, CommentValidation>();
builder.Services.AddScoped<IValidator<RegisterModel>, RegisterValidation>();
builder.Services.AddScoped<IValidator<AuthUser>, AuthUserValidation>();
builder.Services.AddScoped<IValidator<ChangePasswordModel>, ChangePasswordValidation>();
builder.Services.AddTransient<GlobalErrorHandlerMiddleware>();



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{ }
        }
    });
});

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

app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseAuthentication();
app.UseMiddleware<GlobalErrorHandlerMiddleware>();
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

