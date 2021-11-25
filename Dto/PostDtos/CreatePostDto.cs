using Microsoft.AspNetCore.Http;

namespace VAPI.Dto
{
    public class CreatePostDto
    {
        public IFormFile File { get; set; } = null;
        public string Description { get; set; } = null;
        
    }
}