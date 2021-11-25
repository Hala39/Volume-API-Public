using System;

namespace VAPI.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        public AppUser AppUser { get; set; }
        public string AppUserId { get; set; }
        public Post Post { get; set; }
        public int PostId { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        
    }
}