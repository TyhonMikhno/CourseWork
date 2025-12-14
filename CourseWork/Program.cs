using Data;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Repositories.Interfaces;
using Endpoints;
using Validators;

var builder = WebApplication.CreateBuilder(args);

// ---------- DbContext ----------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=music.db"));

// ---------- Repositories ----------
builder.Services.AddScoped<IArtistRepository, ArtistRepository>();
builder.Services.AddScoped<IAlbumRepository, AlbumRepository>();
builder.Services.AddScoped<ITrackRepository, TrackRepository>();
builder.Services.AddScoped<IPlaylistRepository, PlaylistRepository>();

// ---------- FluentValidation ----------
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// ---------- Swagger ----------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ---------- Middleware ----------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ---------- Map Endpoints ----------
app.MapArtistEndpoints();
app.MapAlbumEndpoints();
app.MapTrackEndpoints();
app.MapPlaylistEndpoints();

// ---------- Run ----------
app.Run();
