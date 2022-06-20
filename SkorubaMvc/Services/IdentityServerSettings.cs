namespace SkorubaMvc.Services
{
    // In AppSettings.json
    public class IdentityServerSettings
    {
        public string DiscoveryUrl { get; set; } // Where do we get the discovery document from?
        public string ClientName { get; set; }
        public string ClientPassword { get; set; }
        public bool UseHttps { get; set; }
    }
}
