namespace SocialMedia_API.Data.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public int ConversationId { get; set; }
        public string UserId { get; set; }
        public DateTime SentTime { get; set; }
    }
}
