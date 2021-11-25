using System;

namespace VAPI.Entities
{
    public class SavedPost
    {
        public DateTime SavedAt { get; set; } = DateTime.UtcNow;
        public string SaverId { get; set; }
        public AppUser Saver { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}