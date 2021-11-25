using System;
using Microsoft.AspNetCore.Http;

namespace VAPI.Dto.MessageDtos
{
    public class CreateMessageDto
    {
        public string Content { get; set; }
        public string RecipientId { get; set; }
        public string SenderDisplayName { get; set; }
        public bool Seen { get; set; }
    }
}