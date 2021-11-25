using System;

namespace VAPI.Entities
{
    public class SearchOperation
    {
        public int Id { get; set; }
        public string Keyword { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public AppUser AppUser { get; set; }
        public string AppUserId { get; set; }
        
        
    }
}