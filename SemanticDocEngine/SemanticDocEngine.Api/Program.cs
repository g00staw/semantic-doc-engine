using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SemanticDocEngine.Api.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.UseVector();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.MapScalarApiReference(options =>
    {
        options.Theme = ScalarTheme.Default;
        
        options.WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json");
    });
}

app.Run();