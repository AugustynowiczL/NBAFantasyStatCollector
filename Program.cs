using NBAFantasy.Interfaces;
using NBAFantasy.Services;
using Microsoft.EntityFrameworkCore;
using NBAFantasy.Data;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=NBAFantasyDB.db"));

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddHttpClient<IPlayerService, PlayerService>();
builder.Services.AddHttpClient<IStatsService, StatsService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
