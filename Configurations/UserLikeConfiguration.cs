using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VAPI.Entities;

namespace VAPI.Configurations
{
    public class UserLikeConfiguration : IEntityTypeConfiguration<UserLike>
    {
        public void Configure(EntityTypeBuilder<UserLike> builder)
        {
            builder.HasKey(k => new {k.SourceId, k.PostId});

            builder.HasOne(s => s.Post)
                .WithMany(s => s.Likes)
                .HasForeignKey(s => s.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.Source)
                .WithMany(p => p.LikedPosts)
                .HasForeignKey(s => s.SourceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}