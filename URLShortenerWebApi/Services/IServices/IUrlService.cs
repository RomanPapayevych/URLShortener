using URLShortenerWebApi.Models;

namespace URLShortenerWebApi.Services.IServices
{
    public interface IUrlService
    {
        Task<IEnumerable<UrlOrigin>> GetAllUrlAsync(); 
        Task<UrlOrigin> GetUrlByIdAsync(int id);

        Task<UrlOrigin> CreateShortUrlAsync(string originalUrl, int userId); 
        Task<bool> DeleteUrlAsync(int id, int userId, bool isAdmin);
        Task<UrlOrigin> GetUrlByShortenedAsync(string shortCode);
    }
}
