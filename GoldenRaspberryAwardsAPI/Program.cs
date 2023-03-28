using GoldenRaspberryAwardsAPI.Repository;
using GoldenRaspberryAwardsAPI.Repository.Impl;
using GoldenRaspberryAwardsAPI.Services;
using GoldenRaspberryAwardsAPI.Services.Impl;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ILeituraService, LeituraService>();
builder.Services.AddScoped<IFilmeRepository, FilmeRepository>();

builder.Services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("teste"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
