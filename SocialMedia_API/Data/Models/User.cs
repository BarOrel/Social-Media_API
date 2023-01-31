using Microsoft.AspNetCore.Identity;
using SocialMedia_API.Data.Models.Enums;

namespace SocialMedia_API.Data.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public string About { get; set; }
        public string? Images { get; set; }
        public City ?Location { get; set; }
        public List<User> ?Followers { get; set; }
        public List<User> ?Following { get; set; }
        public List<PostComment> ?Comments { get; set; }
        public List<Conversation> ?Conversations { get; set; }
        public List<Message> ?Messages { get; set; }

    }
}
