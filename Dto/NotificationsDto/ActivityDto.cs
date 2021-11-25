using System;
using VAPI.Entities;

namespace VAPI.Dto.NotificationsDto
{
    public class ActivityDto
    {
        public Guid Id { get; set; }
        public string TargetName { get; set; }
        public string TargetId { get; set; }
        public Stimulation Stimulation { get; set; } 
        public DateTime Date { get; set; }
        public int Path { get; set; }
    }
}