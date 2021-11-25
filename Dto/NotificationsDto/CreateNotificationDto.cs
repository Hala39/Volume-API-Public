using VAPI.Entities;

namespace VAPI.Dto.NotificationsDto
{
    public class CreateNotificationDto
    {        
        public string TargetId { get; set; }        
        public Stimulation Stimulation { get; set; }
        public int Path { get; set; }
        
    }
}