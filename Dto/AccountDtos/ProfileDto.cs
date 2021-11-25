using System.Collections.Generic;
using VAPI.Entities;
using System;

namespace VAPI.Dto
{
    public class ProfileDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Hometown { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public DateTime LastActive { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public string Dob { get; set; }
        public bool IsFollowing { get; set; }
        public DateTime JoinedAt { get; set; }
        
    }
}