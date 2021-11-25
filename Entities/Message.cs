using System;
using System.ComponentModel.DataAnnotations;

namespace VAPI.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public AppUser Sender { get; set; }
        public string SenderId { get; set; }
        public AppUser Recipient { get; set; }
        public string RecipientId { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool Seen { get; set; }
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }
        
    }
}