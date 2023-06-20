using System.Security.Claims;
using CommanderGQL.Data;
using CommanderGQL.GraphQL.Commands;
using CommanderGQL.GraphQL.Platforms;
using CommanderGQL.GrapQL.Platforms;
using CommanderGQL.Models;
using FirebaseAdminAuthentication.DependencyInjection.Models;
using HotChocolate.Authorization;
using HotChocolate.Subscriptions;

namespace CommanderGQL.GrapQL;

public class Mutation
{
    //Platform
    [Authorize(Policy = "IsAdmin")]
    [UseDbContext(typeof(AppDbContext))]
    public async Task<AddPlatformPayload> AddPlatformAsync(
            AddplatformInput input,
            [ScopedService] AppDbContext context,
            [Service] ITopicEventSender eventSender,
            CancellationToken cancellationToken,
            ClaimsPrincipal claimsPrincipal)
    {
        string userId = claimsPrincipal.FindFirstValue(FirebaseUserClaimType.ID);
    
        var platform = new Platform
        {
            Name = input.Name,
            LicenseKey = input.LicenseKey,
            CreatorId = userId
        };

        context.Platforms.Add(platform);
        await context.SaveChangesAsync(cancellationToken);

        await eventSender.SendAsync(nameof(Subscription.OnPlatformAdded), platform, cancellationToken);

        return new AddPlatformPayload(platform);
    }
    
    [Authorize(Policy = "IsAdmin")]
    [UseDbContext(typeof(AppDbContext))]
    public async Task<UpdatePlatformPayload> UpdatePlatformAsync(
            UpdatePlatformInput input,
            [ScopedService] AppDbContext context,
            ClaimsPrincipal claimsPrincipal)
    {
        string userId = claimsPrincipal.FindFirstValue(FirebaseUserClaimType.ID);

        var platform = context.Platforms.FirstOrDefault(p => p.Id == input.id);

        if (platform == null)
        {
            throw new GraphQLException(new Error("Platform not found.", "PLATFORM_NOT_FOUND"));
        }
        else if(platform.CreatorId != userId)
        {
            throw new GraphQLException(new Error("You do not have permission to update this course.", "INVALID_PERMISSION"));
        }

        platform.Name = input.Name;
        platform.LicenseKey = input.LicenseKey;

        context.Platforms.Update(platform);
        await context.SaveChangesAsync();

        return new UpdatePlatformPayload(platform);
    }

    [UseDbContext(typeof(AppDbContext))]
    [Authorize]
    public async Task<DeletePlatformPayload> DeletePlatformAsync(DeletePlatformInput input, [ScopedService] AppDbContext context)
    {
        var platform = context.Platforms.FirstOrDefault(p => p.Id == input.id);
        if (platform == null)
        {
            throw new GraphQLException(new Error("Platform not found.", "PLATFORM_NOT_FOUND"));
        }

        context.Platforms.Remove(platform);
        await context.SaveChangesAsync();

        return new DeletePlatformPayload(true);
    }

    //Command
    [UseDbContext(typeof(AppDbContext))]
    [Authorize]
    public async Task<AddCommandPayload> AddCommandAsync(
        AddCommandInput input, 
        [ScopedService] AppDbContext context,
        [Service] ITopicEventSender eventSender,
        CancellationToken cancellationToken)
    {
        var command = new Command
        {
            HowTo = input.HowTo,
            CommandLine = input.CommandLine,
            PlatformId = input.PlatformId
        };

        context.Commands.Add(command);
        await context.SaveChangesAsync(cancellationToken);

        await eventSender.SendAsync(nameof(Subscription.OnCommandAdded), command, cancellationToken);

        return new AddCommandPayload(command);
    }

    [UseDbContext(typeof(AppDbContext))]
    [Authorize]
    public async Task<UpdateCommandPayload> UpdateCommandAsync(UpdateCommandInput input, [ScopedService] AppDbContext context)
    {
        var command = context.Commands.FirstOrDefault(c => c.Id == input.id);
        if (command == null)
        {
            throw new GraphQLException(new Error("Command not found", "COMMAND_NOT_FOUND"));
        }

        command.HowTo = input.HowTo;
        command.CommandLine = input.CommandLine;
        command.PlatformId = input.PlatformId;

        context.Commands.Update(command);
        await context.SaveChangesAsync();

        return new UpdateCommandPayload(command);
    }

    [UseDbContext(typeof(AppDbContext))]
    [Authorize]
    public async Task<DeleteCommandPayload> DeleteCommandAsync(DeleteCommandInput input, [ScopedService] AppDbContext context)
    {
        var command = context.Commands.FirstOrDefault(c => c.Id == input.Id);
        if(command == null)
        {
            throw new GraphQLException(new Error("Command not found", "COMMAND_NOT_FOUND"));
        }

        context.Commands.Remove(command);
        await context.SaveChangesAsync();

        return new DeleteCommandPayload(true);
    }
}