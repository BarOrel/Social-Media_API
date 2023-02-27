using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialMedia_API.Data.Models.DTO;
using SocialMedia_API.Data.Models;
using SocialMedia_API.Data.Repository.Generic;

namespace SocialMedia_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : Controller
    {
        private readonly IGenericRepository<Post> postRepository;
        private readonly UserManager<User> userManger;
        private readonly IGenericRepository<Like> likeRepository;
        private readonly IGenericRepository<Comment> commentRepository;

        public PostController(IGenericRepository<Post> postRepository,UserManager<User> userManger, IGenericRepository<Like> LikeRepository, IGenericRepository<Comment> commentRepository)
        {
            this.postRepository = postRepository;
            this.userManger = userManger;
            likeRepository = LikeRepository;
            this.commentRepository = commentRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddPost(Post post)
        {
            post.CreatedTime= DateTime.Now;
            await postRepository.Insert(post);
            return Ok(post);
        }

        [HttpPost("Like")]
        public async Task<IActionResult> AddLike(Like like)
        {
            var res = await likeRepository.GetAll();
            var les = res.Any(n => n.UserId == like.UserId && n.PostId == like.PostId);
            if (les == false)
            {
                like.CreatedTime = DateTime.Now;
                await likeRepository.Insert(like);
                return Ok(like);
            }
            var Liked = res.Where(n => n.UserId == like.UserId && n.PostId == like.PostId).FirstOrDefault();
            await likeRepository.Delete(Liked);
            return Ok(false);
        }

        [HttpPost("Like/isLiked")]
        public async Task<IActionResult> IsLiked(Like like)
        {
            var res = await likeRepository.GetAll();
            var Liked = res.Where(n => n.UserId == like.UserId && n.PostId == like.PostId).FirstOrDefault();
            if(Liked != null)
            {
            return Ok(true);

            }
            return Ok(false);
        }




        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            List<PostDTO> Posts = new();
            var res = await postRepository.GetAll();
            foreach (var item in res)
            {
                var user = await userManger.FindByIdAsync(item.UserId);
                PostDTO post = new()
                {
                    Post = item,
                    CommentsCounter =  commentRepository.GetAll().Result.Where(n=>n.PostId == item.Id).Count(),
                    Comments = commentRepository.GetAll().Result.Where(n => n.PostId == item.Id).ToList(),
                    LikesCounter = likeRepository.GetAll().Result.Where(n => n.PostId == item.Id).Count(),
                };
                if (user != null)
                {
                    post.UserImg = user.Images;
                    post.UserId = item.UserId;
                    post.FullName = $"{user.FirstName} {user.LastName}";
                }
                
                Posts.Add(post);
            }
            return Ok(Posts.OrderByDescending(n=>n.Post.CreatedTime));
        }
    }
}
