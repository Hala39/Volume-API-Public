using System;
using VAPI.Entities;

namespace VAPI.Dto.NotificationsDto
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string StimulatorId { get; set; }
        public string StimulatorName { get; set; }
        public string TargetId { get; set; }
        public Stimulation Stimulation { get; set; }
        public int Path { get; set; }
        public DateTime Date { get; set; }
        public bool Seen { get; set; }
        
    }
}