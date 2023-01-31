namespace SocialMedia_API.Data.Models
{
    public class PostComment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string? Images { get; set; }
        public DateTime CreatedTime { get; set; }
        public int Likes { get; set; }
        public Post Post { get; set; }
        public User Author { get; set; }
    }
}
