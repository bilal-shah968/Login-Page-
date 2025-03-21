
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.IdentityModel.Tokens;
// using SchoolManagement.Models;
// using System.Text;

// var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.RequireHttpsMetadata = false;
//         options.SaveToken = true;
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuerSigningKey = true,
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])),
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
//             ValidAudience = builder.Configuration["JwtSettings:Audience"],
//             ClockSkew = TimeSpan.Zero
//         };
//     });

    

// builder.Services.AddControllers();
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowFrontend",
//         policy => policy.WithOrigins("http://localhost:5173")
//                         .AllowAnyMethod()
//                         .AllowAnyHeader()
//                         .AllowCredentials());
// });

// var app = builder.Build();

// app.UseCors("AllowFrontend");
// app.UseAuthentication();
// app.UseAuthorization();
// app.MapControllers();
// app.Run();


using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolManagement.Models;
using SchoolManagement.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Load JWT secret key safely
var jwtSecretKey = builder.Configuration["JwtSettings:SecretKey"];
if (string.IsNullOrEmpty(jwtSecretKey))
{
    throw new ArgumentNullException("JwtSettings:SecretKey", "JWT Secret Key is not configured.");
}

// Configure JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ClockSkew = TimeSpan.Zero
        };
    });

// Add controllers
builder.Services.AddControllers();

// Configure CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:5173")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
});

var app = builder.Build();

// Apply middleware
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();