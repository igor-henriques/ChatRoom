using ChatRoom.ApiService.Application.Commands.AddMessage;
using ChatRoom.ApiService.Application.Commands.CreateChat;
using ChatRoom.ApiService.Application.Commands.DeleteChat;
using ChatRoom.ApiService.Application.Queries.GetChatHistory;
using ChatRoom.ApiService.Application.Queries.GetChats;
using Microsoft.AspNetCore.Mvc;

namespace ChatRoom.ApiService.Endpoints;

public static class ChatEndpoints
{
    public static void ConfigureChatEndpoints(this WebApplication app)
    {
        app.MapGet("/chats", GetChats)
            .RequireAuthorization()
            .WithTags("Chats"); ;

        app.MapPost("/chat", CreateChat)
            .RequireAuthorization()
            .WithTags("Chats"); ;

        app.MapGet("/chat-history", GetChatHistory)
            .RequireAuthorization()
            .WithTags("Messages"); ;

        app.MapPost("/message", AddMessage)
            .RequireAuthorization()
            .WithTags("Messages");

        app.MapDelete("/chat/{chatId}", DeleteChat)
            .RequireAuthorization()
            .WithTags("Chats");
    }

    private static async Task<IResult> GetChats(
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var chats = await mediator.Send(new GetChatsQuery(), cancellationToken);
        return Results.Ok(chats);
    }

    private static async Task<IResult> CreateChat(
        [FromBody] CreateChatCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var chat = await mediator.Send(command, cancellationToken);
        return Results.Ok(chat);
    }

    private static async Task<IResult> GetChatHistory(
        [AsParameters] GetChatHistoryQuery query,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var chatHistory = await mediator.Send(query, cancellationToken);
        return Results.Ok(chatHistory);
    }

    private static async Task<IResult> AddMessage(
       [FromBody] AddMessageCommand command,
       [FromServices] IMediator mediator,
       CancellationToken cancellationToken = default)
    {
        var chatMessage = await mediator.Send(command, cancellationToken);
        return Results.Ok(chatMessage);
    }

    private static async Task<IResult> DeleteChat(
       [FromRoute] Guid chatId,
       [FromServices] IMediator mediator,
       CancellationToken cancellationToken = default)
    {
        _ = await mediator.Send(new DeleteChatCommand(chatId), cancellationToken);
        return Results.NoContent();
    }
}
