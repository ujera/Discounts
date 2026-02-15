using Discounts.API.Infrastructure.Extensions;
using Discounts.API.Infrastructure.Middleware;
using Discounts.Application;
using Discounts.Application.Mappings;
using Discounts.Application.Validators;
using Discounts.Domain.Entities;
using Discounts.Persistance.Context;
using Discounts.Persistance.Seed;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add services
builder.Services.AddServices();

//add dbcontext
builder.Services.AddDbContext<DiscountsManagementContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Discounts.Persistance")
    ));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<DiscountsManagementContext>()
    .AddDefaultTokenProviders();

// 1. Load Settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

//Mapster
MappingConfig.Configure();
builder.Services.AddMapster();

//Validators
builder.Services.AddValidatorsFromAssemblyContaining<CreateOfferValidator>();

var app = builder.Build();
// Global handler(I should left it 1st there)
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using (var scope = app.Services.CreateScope())
    {
        SeedData.Initialize(scope.ServiceProvider);
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
