using System.ComponentModel.DataAnnotations;

namespace VAPI.Dto
{
    public class RegisterDto
    {
        [Required]
        public string DisplayName { get; set; }
        
        [Required]
        public string Email { get; set; }
        [Required]
        // [RegularExpression("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{6,}$", ErrorMessage = "Password must be complex")]
        public string Password { get; set; }
        
    }
}