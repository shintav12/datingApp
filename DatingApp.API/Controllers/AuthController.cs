using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dto;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthRepository repo;
        public AuthController(IAuthRepository repo)
        {
            this.repo = repo;
        }

        [HttpPost("register")]    
        public async Task<IActionResult> Register([FromBody]UserForRegisterDto userForRegisterDto){
             userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if(await repo.UserExists(userForRegisterDto.Username)){
                ModelState.AddModelError("Username", "Username Alrady Exists");
            }

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var userToCreate = new User{
                Username = userForRegisterDto.Username
            };

            var createUser = await repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody]UserForLogingDto userForLoginDto){

            var userFromRepo = repo.Login(userForLoginDto.Username,userForLoginDto.Password);

            if(userFromRepo == null)
                return Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("super secret key");
            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                    new Claim(ClaimTypes.Name, userFromRepo.Username))
                }),
            
            }

        }
    }
}