namespace VAPI.Entities
{
    public class UserLike
    {
        public string SourceId { get; set; }
        public AppUser Source { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        
    }
}