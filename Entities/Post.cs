using System;
using System.Collections.Generic;

namespace VAPI.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public AppUser AppUser { get; set; }
        public string AppUserId { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public File File { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<UserLike> Likes { get; set; } = new List<UserLike>();
        public ICollection<SavedPost> SavedPosts { get; set; } = new List<SavedPost>();
        
        
    }
}