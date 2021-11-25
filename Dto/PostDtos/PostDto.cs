using System;
using System.Collections.Generic;

namespace VAPI.Dto
{
    public class PostDto
    {
        public int Id { get; set; }
        public AppUserDto AppUser { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public FileDto File { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public bool IsLikedByUser { get; set; }
        public bool IsFollowing { get; set; }
        public bool IsSavedByUser { get; set; }
        
        
    }
}