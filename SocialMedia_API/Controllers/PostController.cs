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
            post.CreatedTime= DateTime.Now;
            await postRepository.Insert(post);
            return Ok(post);
        }


        [HttpPost("Comment")]
        public async Task<IActionResult> AddComment(Comment comment)
        {
            comment.CreatedTime = DateTime.Now;
            await commentRepository.Insert(comment);
            return Ok(comment);
          
        }

        [HttpPost("Like")]
        public async Task<IActionResult> AddLike(Like like)
        {
            var res = await likeRepository.GetAll();
            var les = res.Any(n => n.UserId == like.UserId && n.PostId == like.PostId);
            if (les == false)
            {
                Notification notification = new()
                {
                    CreatedTime = DateTime.Now,
                    NotifierId = like.UserId,
                    PostId= like.PostId,
                    Type = NotificationType.Like   
                };
                notification.UserId = postRepository.GetById(like.PostId).Result.UserId;
                
                if (notification.UserId != notification.NotifierId)
                    await notificationRepository.Insert(notification);
                

                
                like.CreatedTime = DateTime.Now;
                await likeRepository.Insert(like);
                return Ok(true);
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
        
        [HttpGet("GetById/{Id}")]
        public async Task<IActionResult> GetPostById(int Id)
        {
            var post = await postService.GetPostDtoById(Id);
            if (post != null)
                return Ok(post);
            
            else { return BadRequest(); }
           
        }



        [HttpGet]
        public async Task<IActionResult> Explore()
        {
            List<PostDTO> Posts = new();
            var res = await postRepository.GetAll();
            foreach (var item in res)
            {
                List<CommentDTO> Comments = new();
                var user = await userManger.FindByIdAsync(item.UserId);
                PostDTO post = new()
                {
                    Post = item,
                    CommentsCounter =  commentRepository.GetAll().Result.Where(n=>n.PostId == item.Id).Count(),
                    LikesCounter = likeRepository.GetAll().Result.Where(n => n.PostId == item.Id).Count(),
                };
                
                var comments = commentRepository.GetAll().Result.Where(n => n.PostId == item.Id).ToList();
                foreach (var comment in comments)
                {
                    var commentUser = await userManger.FindByIdAsync(comment.UserId);
                    CommentDTO dtocomment = new();
                    dtocomment.Comment = comment;
                    if (commentUser != null)
                    {
                        dtocomment.UserFullName = $"{commentUser.FirstName} {commentUser.LastName}";
                        dtocomment.UserImg = commentUser.Images;
                    }

                    Comments.Add(dtocomment);
                }

                post.Comments = Comments.OrderByDescending(n=>n.Comment.CreatedTime).ToList();


                if (user != null)
                {
                    post.UserImg = user.Images;
                    post.UserId = item.UserId;
                    post.FullName = $"{user.FirstName} {user.LastName}";
                }
                
                Posts.Add(post);
            }
            return Ok(Posts.OrderByDescending(n=>n.LikesCounter));
        }


        [HttpGet("GetFollowingPosts/{UserId}")]
        public async Task<IActionResult> GetPosts(string UserId)
        {

            List<PostDTO> Posts = new();
            var res = await postRepository.GetAll();
            var followers = await followRepository.GetAll();

            var posts = res
                 .Where(p => followers
                   .Where(f => f.FollowerId == UserId)
                    .Select(f => f.FollowingId)
                        .Contains(p.UserId))
                            .ToList();

            posts.AddRange(res.Where(n => n.UserId == UserId));
           


            foreach (var item in posts)
            {
                var post = await postService.GetPostDtoById(item.Id);
                Posts.Add(post);
            }

            return Ok(Posts.OrderByDescending(n=>n.Post.CreatedTime));
        }
    }
}
