using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using URLShortenerWebApi.Models;
using URLShortenerWebApi.Services.IServices;

namespace URLShortenerWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _userRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AuthController(IAuthService userRepository, SignInManager<ApplicationUser> signInManager)
        {
            _userRepository = userRepository;
            _signInManager = signInManager;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid state");
            }
            var user = await _userRepository.Login(login);
            if (user.Succeeded)
            {
                var tokenString = _userRepository.GenerateToken(login);
                return Ok(tokenString);
            }
            return BadRequest("Invalid email or password");
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register registerDTO)
        {
            if (ModelState.IsValid)
            {
                bool ifUnique = _userRepository.isUniqueUser(registerDTO.Name!);
                if (!ifUnique)
                {
                    return BadRequest("Username already exist");
                }
                var register = await _userRepository.Register(registerDTO);
                return Ok(register);
            }
            return BadRequest("Error");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (ModelState.IsValid)
            {
                await _signInManager.SignOutAsync();
                return Ok("Logout");
            }
            return BadRequest("Not logout");
        }
    }
}
