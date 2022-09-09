using CRUDApi.Models;
using CRUDApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using CRUDApi.Requests;
using FluentValidation;
using CRUDApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IBookService, BookService>();
builder.Services.AddValidatorsFromAssemblyContaining(typeof(BookRequestValidator));
builder.Services.AddDbContext<BooksDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


var app = builder.Build();

app.UseSwagger();
app.UseAuthorization();
app.UseAuthentication();

app.MapPost("/login", async (BooksDbContext context, string username, string password) => await Login(context, username, password));
app.RegisterEndpoins();

async Task<IResult> Login(BooksDbContext context, string username, string password)
{
    if (username != null && password != null)
    {
        var loggedInUser = await context.Users.FirstOrDefaultAsync(c => c.Username == username && c.Password == password);
        if (loggedInUser is null) return Results.NotFound("User not found");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, loggedInUser.Username),
            new Claim(ClaimTypes.Email, loggedInUser.Email),
            new Claim(ClaimTypes.Name, loggedInUser.Name),
            new Claim(ClaimTypes.Surname, loggedInUser.Surname),
            new Claim(ClaimTypes.Role, loggedInUser.Role.ToLower())
        };

        var token = new JwtSecurityToken
        (
            issuer: builder.Configuration["Jwt:Issuer"],
            audience: builder.Configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(30),
            notBefore: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                SecurityAlgorithms.HmacSha256)
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return Results.Ok(tokenString);
    }
    return Results.NotFound();
    
}

app.UseSwaggerUI();

app.Run();
