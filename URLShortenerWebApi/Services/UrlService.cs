using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using URLShortenerWebApi.Data;
using URLShortenerWebApi.Models;
using URLShortenerWebApi.Services.IServices;

namespace URLShortenerWebApi.Services
{
    public class UrlService : IUrlService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UrlService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IEnumerable<UrlOrigin>> GetAllUrlAsync()
        {
            return await _context.Urls.ToListAsync(); 
        }
        public async Task<UrlOrigin> GetUrlByIdAsync(int id)
        {
            var getUrl = await _context.Urls.FindAsync(id);
            return getUrl!;
        }
        public async Task<UrlOrigin> CreateShortUrlAsync(string Url, int userId)
        {
            var createShortUrl = await _context.Urls.AnyAsync(u => u.Url == Url);
            if (createShortUrl)
            {
                throw new InvalidOperationException("URL already exists");
            }
            string shortCode = GenerateShortCode();
            var shortUrl = new UrlOrigin
            {
                Url = Url,
                ShortUrl = shortCode,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            _context.Urls.Add(shortUrl);
            await _context.SaveChangesAsync();
            return shortUrl;
        }

        public async Task<bool> DeleteUrlAsync(int id, int userId, bool isAdmin)
        {
            var url = await _context.Urls.FirstOrDefaultAsync(u => u.id == id);

            if (url == null)
            {
                return false; 
            }
            if (url.UserId != userId && !isAdmin)
            {
                throw new UnauthorizedAccessException("You cannot delete this entry");
            }
            _context.Urls.Remove(url);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UrlOrigin> GetUrlByShortenedAsync(string shortCode)
        {
            return (await _context.Urls.FirstOrDefaultAsync(u => u.ShortUrl == shortCode))!;
        }
        private string GenerateShortCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
