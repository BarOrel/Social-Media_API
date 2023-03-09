using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialMedia_API.Data.Models.DTO;
using SocialMedia_API.Data.Models;
using SocialMedia_API.Data.Repository.Generic;
using SocialMedia_API.Services.PostService;
using System.Linq;
using SocialMedia_API.Data.Models.Enums;

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
        private readonly IPostService postService;
        private readonly IGenericRepository<Follow> followRepository;
        private readonly IGenericRepository<Notification> notificationRepository;

        public PostController(IGenericRepository<Post> postRepository
            ,UserManager<User> userManger, IGenericRepository<Like> LikeRepository
            , IGenericRepository<Comment> commentRepository,IPostService postService
            , IGenericRepository<Follow> followRepository
            , IGenericRepository<Notification> notificationRepository)
        {
            this.postRepository = postRepository;
            this.userManger = userManger;
            likeRepository = LikeRepository;
            this.commentRepository = commentRepository;
            this.postService = postService;
            this.followRepository = followRepository;
            this.notificationRepository = notificationRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddPost(Post post)
        {
            await postRepository.Insert(post);
            return Ok(post);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await postRepository.GetById(id);
            if (post != null)
            {
                var notifications = await notificationRepository.GetAll();
                foreach (var item in notifications)  { await notificationRepository.Delete(item); }
                await postRepository.Delete(post);
                return Ok(post);
            }
            return BadRequest();
        }

        [HttpGet("GetById/{Id}")]
        public async Task<IActionResult> GetPostById(int Id)
        {
            var post = await postService.GetPostDtoById(Id);
            if (post != null)
                return Ok(post);
            
            return BadRequest(); 
           
        }

        [HttpGet]
        public async Task<IActionResult> Explore()
        {
            var Posts = await postService.GetExplore();
            if (Posts != null)
            {
                return Ok(Posts.OrderByDescending(n=>n.LikesCounter));
            }

            return BadRequest(); 
        }

        [HttpGet("GetFollowingPosts/{UserId}")]
        public async Task<IActionResult> GetFollowingPosts(string UserId)
        {
            List<PostDTO> Posts = new();
            var allPosts = await postRepository.GetAll();
            var followers = await followRepository.GetAll();
            var posts = allPosts
                 .Where(p => followers
                   .Where(f => f.FollowerId == UserId)
                    .Select(f => f.FollowingId)
                        .Contains(p.UserId))
                            .ToList();

            posts.AddRange(allPosts.Where(n => n.UserId == UserId));
           
            foreach (var item in posts)
            {
                var post = await postService.GetPostDtoById(item.Id);
                Posts.Add(post);
            }

            return Ok(Posts.OrderByDescending(n=>n.Post.CreatedTime));
        }

        [HttpPost("Like")]
        public async Task<IActionResult> AddLike(Like like)
        {
            var allLikes = await likeRepository.GetAll();
            var les = allLikes.Any(n => n.UserId == like.UserId && n.PostId == like.PostId);
            if (les == false)
            {
                Notification notification = new()
                {
                    NotifierId = like.UserId,
                    PostId= like.PostId,
                    Type = NotificationType.Like   
                };
                notification.UserId = postRepository.GetById(like.PostId).Result.UserId;
                
                if (notification.UserId != notification.NotifierId)
                    await notificationRepository.Insert(notification);
                     
                await likeRepository.Insert(like);
                return Ok(true);

            }
            var Liked = allLikes.Where(n => n.UserId == like.UserId && n.PostId == like.PostId).FirstOrDefault();
            await likeRepository.Delete(Liked);

            return Ok(false);
        }

        [HttpPost("Like/isLiked")]
        public async Task<IActionResult> IsLiked(Like like)
        {
            var allLikes = await likeRepository.GetAll();
            var Liked = allLikes.Where(n => n.UserId == like.UserId && n.PostId == like.PostId).FirstOrDefault();
            if(Liked != null)
            {
                 return Ok(true);

            }
            return Ok(false);
        }
    }
}
