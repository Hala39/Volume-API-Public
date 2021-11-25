namespace VAPI.Helpers
{
    public class MessageParams : PaginationParams
    {
        public string UserId { get; set; }
        public string Container { get; set; } = "Unread";
    }
}