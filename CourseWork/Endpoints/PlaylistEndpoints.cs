using DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repositories.Interfaces;

namespace Endpoints;

public static class PlaylistEndpoints
{
    public static void MapPlaylistEndpoints(this WebApplication app)
    {
        app.MapGet("/playlists", async (IPlaylistRepository repo) => await repo.GetAllAsync());

        app.MapGet("/playlists/{id:int}", async (int id, IPlaylistRepository repo) =>
        {
            var playlist = await repo.GetByIdAsync(id);
            return playlist is not null ? Results.Ok(playlist) : Results.NotFound();
        });

        app.MapPost("/playlists", async (PlaylistDto dto, IPlaylistRepository repo, IValidator<PlaylistDto> validator) =>
        {
            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid) return Results.BadRequest(validation.Errors);

            var playlist = new Playlist
            {
                Name = dto.Name!,
                Description = dto.Description
            };
            await repo.CreateAsync(playlist);
            return Results.Created($"/playlists/{playlist.Id}", playlist);
        });

        app.MapPut("/playlists/{id:int}", async (int id, PlaylistDto dto, IPlaylistRepository repo, IValidator<PlaylistDto> validator) =>
        {
            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid) return Results.BadRequest(validation.Errors);

            var playlist = await repo.GetByIdAsync(id);
            if (playlist is null) return Results.NotFound();

            playlist.Name = dto.Name!;
            playlist.Description = dto.Description;

            await repo.UpdateAsync(playlist);
            return Results.Ok(playlist);
        });

        app.MapDelete("/playlists/{id:int}", async (int id, IPlaylistRepository repo) =>
        {
            await repo.DeleteAsync(id);
            return Results.NoContent();
        });
    }
}
