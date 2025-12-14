using DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repositories.Interfaces;

namespace Endpoints;

public static class ArtistEndpoints
{
    public static void MapArtistEndpoints(this WebApplication app)
    {
        app.MapGet("/artists", async (IArtistRepository repo) =>
            await repo.GetAllAsync());

        app.MapGet("/artists/{id:int}", async (int id, IArtistRepository repo) =>
        {
            var artist = await repo.GetByIdAsync(id);
            return artist is not null ? Results.Ok(artist) : Results.NotFound();
        });

        app.MapPost("/artists", async (ArtistDto dto, IArtistRepository repo, IValidator<ArtistDto> validator) =>
        {
            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid)
                return Results.BadRequest(validation.Errors);

            var artist = new Artist
            {
                Name = dto.Name!,
                Age = dto.Age,
                Country = dto.Country!,
                Label = dto.Label!
            };
            await repo.CreateAsync(artist);
            return Results.Created($"/artists/{artist.Id}", artist);
        });

        app.MapPut("/artists/{id:int}", async (int id, ArtistDto dto, IArtistRepository repo, IValidator<ArtistDto> validator) =>
        {
            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid)
                return Results.BadRequest(validation.Errors);

            var artist = await repo.GetByIdAsync(id);
            if (artist is null) return Results.NotFound();

            artist.Name = dto.Name!;
            artist.Age = dto.Age;
            artist.Country = dto.Country!;
            artist.Label = dto.Label!;

            await repo.UpdateAsync(artist);
            return Results.Ok(artist);
        });

        app.MapDelete("/artists/{id:int}", async (int id, IArtistRepository repo) =>
        {
            await repo.DeleteAsync(id);
            return Results.NoContent();
        });
    }
}
