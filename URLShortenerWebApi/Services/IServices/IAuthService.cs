using URLShortenerWebApi.Models;

namespace URLShortenerWebApi.Services.IServices
{
    public interface IAuthService
    {
        bool isUniqueUser(string name);
        string GenerateToken(Login login);
        Task<OperationResult> Register(Register register);
        Task<OperationResult> Login(Login login);
        Task<OperationResult> DeleteUser(string name);
    }

}
