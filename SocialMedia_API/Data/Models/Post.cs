namespace SocialMedia_API.Data.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedTime { get; set; }
        public string? Images { get; set; }
        public List<PostComment> ?Comments { get; set; }
        public string AuthorUserId { get; set; }
        public List<User> ?LikedByUsers { get; set; }

    }
}
