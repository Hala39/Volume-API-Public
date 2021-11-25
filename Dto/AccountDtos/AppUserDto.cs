using System;

namespace VAPI.Dto
{
    public class AppUserDto
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public DateTime LastActive { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public bool IsFollowing { get; set; }
       public MessageDto LastMessageSent { get; set; }
       public MessageDto LastMessageReceived { get; set; }
       
       
    }
}