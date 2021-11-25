using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VAPI.Entities;

namespace VAPI.Configurations
{
    public class BookmarkConfiguration : IEntityTypeConfiguration<SavedPost>
    {
        public void Configure(EntityTypeBuilder<SavedPost> builder)
        {
            builder.HasKey(k => new {k.SaverId, k.PostId});

            builder.HasOne(s => s.Post)
                .WithMany(s => s.SavedPosts)
                .HasForeignKey(s => s.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.Saver)
                .WithMany(p => p.SavedPosts)
                .HasForeignKey(s => s.SaverId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}