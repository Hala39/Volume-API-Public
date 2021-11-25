using System;

namespace VAPI.Entities
{
    public class File
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public AppUser AppUser { get; set; }
        public string AppUserId { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        
        
        
    }
}