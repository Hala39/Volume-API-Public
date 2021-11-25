using System;

namespace VAPI.Dto
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public AppUserDto AppUser { get; set; }
        public int PostId { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}