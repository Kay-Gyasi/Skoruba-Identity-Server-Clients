using IdentityModel.Client;
using Microsoft.Extensions.Options;

namespace SkorubaMvc.Services
{
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly IOptions<IdentityServerSettings> _identityServerSettings;
        private readonly DiscoveryDocumentResponse _discoveryDocument;

        public TokenService(ILogger<TokenService> logger, IOptions<IdentityServerSettings> identityServerSettings)
            // configure IOptions in startup.cs
        {
            _logger = logger;
            _identityServerSettings = identityServerSettings;

            // We need the discovery document to get the endpoint for generating the tokens
            using var httpClient = new HttpClient();
            _discoveryDocument = httpClient.GetDiscoveryDocumentAsync(_identityServerSettings.Value.DiscoveryUrl).Result;
            if (_discoveryDocument.IsError)
            {
                _logger.LogError($"Error retrieving discovery document: {_discoveryDocument.Error}");
                throw new Exception("Unable to get discovery document", _discoveryDocument.Exception);
            }
        }
        
        public async Task<TokenResponse> GetToken(string scope)
        {
            using var client = new HttpClient();
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = _discoveryDocument.TokenEndpoint,
                ClientId = _identityServerSettings.Value.ClientName,
                ClientSecret = _identityServerSettings.Value.ClientPassword,
                Scope = scope
            });

            if (tokenResponse.IsError)
            {
                _logger.LogError($"Error retrieving token: {tokenResponse.Error}");
                throw new Exception("Unable to get token", tokenResponse.Exception);
            }

            return tokenResponse;
        }
    }
}
