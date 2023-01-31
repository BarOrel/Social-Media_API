namespace SocialMedia_API.Data.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? ImgUrl { get; set; }
        public List<User> Users { get; set; }
    }
}
