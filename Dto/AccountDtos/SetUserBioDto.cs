using Microsoft.AspNetCore.Http;

namespace VAPI.Dto
{
    public class SetUserBioDto
    {
        public string Title { get; set; } = null;
        public string Gender { get; set; } = null;
        public string Dob { get; set; } = null;
        public string Hometown { get; set; } = null;
        public string DisplayName { get; set; } 
        public string PhoneNumber { get; set; } = null;
            
    }
}