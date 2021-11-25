using System;

namespace VAPI.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public AppUser Target { get; set; }
        public string TargetId { get; set; }
        public AppUser Stimulator { get; set; }
        public string StimulatorId { get; set; }
        public Stimulation Stimulation { get; set; } 
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public bool Seen { get; set; } = false;
        public int Path { get; set; }
        public bool TargetDeleted { get; set; }
        public bool StimulatorDeleted { get; set; }
        
        
    }
}