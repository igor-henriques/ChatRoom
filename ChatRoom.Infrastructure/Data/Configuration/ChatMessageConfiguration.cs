using ChatRoom.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatRoom.Infrastructure.Data.Configuration;

public sealed class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable("ChatMessages");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.ChatId)
            .IsRequired();

        builder.Property(m => m.Content)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(m => m.CreatedDate)
            .IsRequired();

        builder.Property(c => c.CreatedByUserId)
           .IsRequired()
           .HasMaxLength(450);

        builder.HasOne<Chat>()
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.CreatedByUser)
            .WithMany()
            .HasForeignKey(c => c.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
