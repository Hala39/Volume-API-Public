using System;

namespace VAPI.Dto
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string SenderDisplayName { get; set; }
        public string SenderId { get; set; }
        public string RecipientId { get; set; }
        public DateTime SentAt { get; set; } 
        public bool Seen { get; set; }
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }
        
    }
}