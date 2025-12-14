using DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repositories.Interfaces;

namespace Endpoints;

public static class TrackEndpoints
{
    public static void MapTrackEndpoints(this WebApplication app)
    {
        app.MapGet("/tracks", async (ITrackRepository repo) => await repo.GetAllAsync());

        app.MapGet("/tracks/{id:int}", async (int id, ITrackRepository repo) =>
        {
            var track = await repo.GetByIdAsync(id);
            return track is not null ? Results.Ok(track) : Results.NotFound();
        });

        app.MapPost("/tracks", async (TrackDto dto, ITrackRepository repo, IValidator<TrackDto> validator) =>
        {
            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid) return Results.BadRequest(validation.Errors);

            var track = new Track
            {
                Title = dto.Title!,
                DurationSeconds = dto.DurationSeconds,
                Genre = dto.Genre!
            };
            await repo.CreateAsync(track);
            return Results.Created($"/tracks/{track.Id}", track);
        });

        app.MapPut("/tracks/{id:int}", async (int id, TrackDto dto, ITrackRepository repo, IValidator<TrackDto> validator) =>
        {
            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid) return Results.BadRequest(validation.Errors);

            var track = await repo.GetByIdAsync(id);
            if (track is null) return Results.NotFound();

            track.Title = dto.Title!;
            track.DurationSeconds = dto.DurationSeconds;
            track.Genre = dto.Genre!;

            await repo.UpdateAsync(track);
            return Results.Ok(track);
        });

        app.MapDelete("/tracks/{id:int}", async (int id, ITrackRepository repo) =>
        {
            await repo.DeleteAsync(id);
            return Results.NoContent();
        });
    }
}
