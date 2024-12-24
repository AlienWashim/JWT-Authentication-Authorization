using Azure.Core;
using JWTAuthenticationAuthorization;
using JWTAuthenticationAuthorization.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<UserService>();  // Register UserService
builder.Services.AddScoped<TokenGenerator>();  // Register TokenGenerator
// Register MyDBContext with a connection string (or connection options)
builder.Services.AddDbContext<MyDBContext>(options =>
    options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=JWTUser;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/login", async (LoginRequest request, TokenGenerator tokenGenerator) =>
{
    var token = await tokenGenerator.GenerateTokenAsync(request.Email, request.Password);
    return new { access_token = token };
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
