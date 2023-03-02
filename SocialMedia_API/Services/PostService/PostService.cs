using Microsoft.AspNetCore.Identity;
using SocialMedia_API.Data.Models;
using SocialMedia_API.Data.Models.DTO;
using SocialMedia_API.Data.Repository.Generic;

namespace SocialMedia_API.Services.PostService
{
    public class PostService : IPostService
    {
        private readonly IGenericRepository<Post> postRepository;
        private readonly UserManager<User> userManger;
        private readonly IGenericRepository<Like> likeRepository;
        private readonly IGenericRepository<Comment> commentRepository;

        public PostService(IGenericRepository<Post> postRepository, UserManager<User> userManger, IGenericRepository<Like> LikeRepository, IGenericRepository<Comment> commentRepository)
        {
            this.postRepository = postRepository;
            this.userManger = userManger;
            likeRepository = LikeRepository;
            this.commentRepository = commentRepository;
        }

        public async Task<PostDTO> GetPostDtoById(int Id)
        {
            PostDTO post = new();
            List<CommentDTO> Comments = new();
            var res = await postRepository.GetById(Id);
            var user = await userManger.FindByIdAsync(res.UserId);

            post.Post = res;
            //post.Comments = commentRepository.GetAll().Result.Where(n => n.PostId == res.Id).ToList();
            var comments = commentRepository.GetAll().Result.Where(n => n.PostId == res.Id).ToList();
            foreach (var item in comments)
            {
                var commentUser = await userManger.FindByIdAsync(item.UserId);
                CommentDTO dtocomment = new();
                dtocomment.Comment = item;
                if (commentUser != null)
                {
                    dtocomment.UserFullName = $"{commentUser.FirstName} {commentUser.LastName}";
                    dtocomment.UserImg = commentUser.Images;
                }

                Comments.Add(dtocomment);
            }

            post.Comments = Comments.OrderByDescending(n => n.Comment.CreatedTime).ToList();
            post.CommentsCounter = commentRepository.GetAll().Result.Where(n => n.PostId == res.Id).Count();
            post.LikesCounter = likeRepository.GetAll().Result.Where(n => n.PostId == res.Id).Count();

            if (user != null)
            {
                post.FullName = $"{user.FirstName} {user.LastName}";
                post.UserId = user.Id;
                post.UserImg = user.Images;
            }
            return post;
        }
    }
}
