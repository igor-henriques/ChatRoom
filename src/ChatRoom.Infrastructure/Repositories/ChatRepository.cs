using ChatRoom.Domain.Entities;
using ChatRoom.Domain.Repositories;
using ChatRoom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatRoom.Infrastructure.Repositories;

public sealed class ChatRepository : IChatRepository
{
    private readonly ApplicationDbContext _context;

    public ChatRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Chat> AddChatAsync(Chat chat, CancellationToken cancellationToken = default)
    {
        _context.Chats.Add(chat);
        await _context.SaveChangesAsync(cancellationToken);
        return chat;
    }

    public async Task<ChatMessage> AddChatMessageAsync(ChatMessage chatMessage, CancellationToken cancellationToken = default)
    {
        _context.ChatMessages.Add(chatMessage);
        await _context.SaveChangesAsync(cancellationToken);
        return chatMessage;
    }

    public async Task DeleteChatAsync(Guid chatId, CancellationToken cancellationToken = default)
    {
        await _context.Chats.Where(x => x.Id == chatId).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task DeleteChatMessageAsync(Guid chatMessageId, CancellationToken cancellationToken = default)
    {
        await _context.ChatMessages.Where(x => x.Id == chatMessageId).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<Chat?> GetChatAsync(Guid chatId, CancellationToken cancellationToken = default)
    {
        return await _context
            .Chats
            .Include(x => x.Messages)
            .ThenInclude(x => x.CreatedByUser)
            .AsNoTracking()
            .Where(x => x.Id == chatId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ChatMessage?> GetChatMessageAsync(Guid chatMessageId, CancellationToken cancellationToken = default)
    {
        return await _context
            .ChatMessages
            .Include(x => x.CreatedByUser)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == chatMessageId, cancellationToken);
    }

    public async Task<IEnumerable<ChatMessage?>> GetChatMessagesAsync(Guid chatId, int historySize, CancellationToken cancellationToken = default)
    {
        return await _context
           .ChatMessages
           .Include(x => x.CreatedByUser)
           .AsNoTracking()
           .Where(x => x.ChatId == chatId)
           .OrderBy(x => x.CreatedDate)
           .Take(historySize)
           .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Chat?>> GetChatsAsync(CancellationToken cancellationToken = default)
    {
        return await _context
            .Chats
            .AsNoTracking()
            .Include(x => x.CreatedByUser)
            .ToListAsync(cancellationToken);
    }
}