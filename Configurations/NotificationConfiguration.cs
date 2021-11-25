using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VAPI.Entities;

namespace VAPI.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasOne(u => u.Target)
                .WithMany(m => m.Notifications)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.Stimulator)
                .WithMany(m => m.Activity)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}