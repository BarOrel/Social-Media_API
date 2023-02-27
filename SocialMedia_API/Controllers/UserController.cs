using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialMedia_API.Data.Models;
using SocialMedia_API.Data.Models.DTO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialMedia_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> userManager;

        public UserController(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            User user = new()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.UserName,
                Gender = model.Gender,
                
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if(result.Succeeded) 
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> NavbarView(string UserId)
        {
            var user = await userManager.FindByIdAsync(UserId);
            if (user != null)
            {
            NavbarDTO view = new()
            {
                UserId = UserId,
                ImgUrl = user.Images,
                FullName = $"{user.FirstName} {user.LastName}"

            };
                return Ok(view);
            }
            return BadRequest();
        }





    }
}
