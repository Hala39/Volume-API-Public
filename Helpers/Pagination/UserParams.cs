namespace VAPI.Helpers
{
    public class UserParams : PaginationParams
    {
        public string Keyword { get; set; }
        public bool Suggest { get; set; } = false;
        
    }
}