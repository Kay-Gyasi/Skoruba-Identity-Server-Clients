using IdentityModel.Client;

namespace SkorubaMvc.Services
{
    public interface ITokenService
    {
        // TokenResponse contains the details of the token
        Task<TokenResponse> GetToken(string scope);
    }
}
