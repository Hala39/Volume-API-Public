using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace VAPI.Entities
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public string Title { get; set; }
        public string Dob { get; set; }
        public string Hometown { get; set; }
        public string Gender { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public List<SearchOperation> SearchOperations { get; set; } 
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public ICollection<Post> Posts { get; set; }
        public ICollection<File> Files { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<UserLike> LikedPosts { get; set; }
        public ICollection<UserFollowing> Followers { get; set; }
        public ICollection<UserFollowing> Followings { get; set; }
        public ICollection<Message> MessagesSent { get; set; } = new List<Message>();
        public ICollection<Message> MessagesReceived { get; set; } = new List<Message>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<Notification> Activity { get; set; } = new List<Notification>();
        public ICollection<SavedPost> SavedPosts { get; set; } = new List<SavedPost>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        
        
        
        
    }
}