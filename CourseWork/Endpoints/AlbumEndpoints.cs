using DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repositories.Interfaces;

namespace Endpoints;

public static class AlbumEndpoints
{
    public static void MapAlbumEndpoints(this WebApplication app)
    {
        app.MapGet("/albums", async (IAlbumRepository repo) => await repo.GetAllAsync());

        app.MapGet("/albums/{id:int}", async (int id, IAlbumRepository repo) =>
        {
            var album = await repo.GetByIdAsync(id);
            return album is not null ? Results.Ok(album) : Results.NotFound();
        });

        app.MapPost("/albums", async (AlbumDto dto, IAlbumRepository repo, IValidator<AlbumDto> validator) =>
        {
            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid) return Results.BadRequest(validation.Errors);
            
            var album = new Album
            {
                Title = dto.Title!,
                Length = dto.Length,
                Year = dto.Year,
                SongCount = dto.SongCount,
                Genre = dto.Genre!
            };
            await repo.CreateAsync(album);
            return Results.Created($"/albums/{album.Id}", album);
        });

        app.MapPut("/albums/{id:int}", async (int id, AlbumDto dto, IAlbumRepository repo, IValidator<AlbumDto> validator) =>
        {
            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid) return Results.BadRequest(validation.Errors);

            var album = await repo.GetByIdAsync(id);
            if (album is null) return Results.NotFound();

            album.Title = dto.Title!;
            album.Length = dto.Length;
            album.Year = dto.Year;
            album.SongCount = dto.SongCount;
            album.Genre = dto.Genre!;

            await repo.UpdateAsync(album);
            return Results.Ok(album);
        });

        app.MapDelete("/albums/{id:int}", async (int id, IAlbumRepository repo) =>
        {
            await repo.DeleteAsync(id);
            return Results.NoContent();
        });
    }
}
