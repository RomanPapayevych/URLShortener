using Microsoft.AspNetCore.Identity;

namespace URLShortenerWebApi.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string? PersonName { get; set; }
        public ICollection<UrlOrigin>? Urls { get; set; }
    }
}
