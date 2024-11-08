using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using URLShortenerWebApi.Data;
using URLShortenerWebApi.Models;
using URLShortenerWebApi.Services.IServices;

namespace URLShortenerWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlController : ControllerBase
    {
        private readonly IUrlService _urlService;
        public UrlController(IUrlService urlService)
        {
            _urlService = urlService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UrlOrigin>>> GetUrls()
        {
            if (ModelState.IsValid)
            {

                return Ok(await _urlService.GetAllUrlAsync());
            }
            return BadRequest("Url not exist");
        }

        [Authorize]
        [HttpPost("shorten")]
        public async Task<ActionResult<UrlOrigin>> CreateShortUrl([FromBody] string urlOrigin)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(urlOrigin))
                {
                    return BadRequest("URL can't be blank");
                }
                else
                {
                    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                    var shortUrl = await _urlService.CreateShortUrlAsync(urlOrigin, userId);
                    return Ok(shortUrl);
                }
            }
            return BadRequest("Invalid operation");
        }

        [HttpGet("${shortCode}")]
        public async Task<IActionResult> RedirectToOriginalUrl(string shortCode) 
        { 
            if (ModelState.IsValid)
            {
                var findUrl = await _urlService.GetUrlByShortenedAsync(shortCode);
                if (findUrl == null)
                {
                    return NotFound();
                }
                return Ok(new { url = findUrl.Url });
            }
            return BadRequest("Something go wrong");
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUrl(int id)
        {
            if (ModelState.IsValid)
            {
                var getUrl = await _urlService.GetUrlByIdAsync(id);
                return Ok(getUrl);
            }
            return BadRequest("Invalid id");
            
        }
        [Authorize]
        [HttpDelete("url/{id}")]
        public async Task<IActionResult> DeleteUrl(int id)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var isAdmin = User.IsInRole("Admin");
                var url = await _urlService.GetUrlByIdAsync(id);
                if (url == null)
                {
                    return NotFound();
                }
                if (url.UserId != userId && !isAdmin)
                {
                    throw new UnauthorizedAccessException("You cannot delete this entry");
                }
                await _urlService.DeleteUrlAsync(id, userId, isAdmin);
                return Ok("Deleted");
            }
            return BadRequest("Invalid deleting");
        }
        
    }
}
