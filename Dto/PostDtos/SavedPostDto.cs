using System;

namespace VAPI.Dto
{
    public class SavedPostDto
    {
        public int Id { get; set; }
        public string OwnerDisplayName { get; set; }
        public string Description { get; set; }
        public DateTime SavedAt { get; set; }
        public FileDto File { get; set; }
    }
}