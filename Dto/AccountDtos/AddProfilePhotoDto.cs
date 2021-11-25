using Microsoft.AspNetCore.Http;

namespace VAPI.Dto.AccountDtos
{
    public class AddProfilePhotoDto
    {
        public IFormFile File { get; set; } = null;
        public string Url { get; set; } = null;
        
    }
}