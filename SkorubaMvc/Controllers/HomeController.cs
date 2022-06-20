using Microsoft.AspNetCore.Mvc;
using SkorubaMvc.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using SkorubaMvc.Services;

namespace SkorubaMvc.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(ILogger<HomeController> logger,
            ITokenService tokenService,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            ViewData["Username"] = _httpContextAccessor.HttpContext?.User.FindFirst("name")?.Value;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //[Authorize]
        public async Task<IActionResult> Weather()
        {
            using var client = new HttpClient();

            //var token = await _tokenService.GetToken("skoruba_api_read"); // For getting token from identity server
            //client.SetBearerToken(token.AccessToken);

            var token = await HttpContext.GetTokenAsync("access_token");
            client.SetBearerToken(token);

            var result = await client.GetAsync("https://localhost:7043/weatherforecast");

            if (result.IsSuccessStatusCode)
            {
                // Deserialize json result into WeatherData[] (Add package Newtonsoft.Json)
                var model = await result.Content.ReadAsStringAsync();

                var data = JsonConvert.DeserializeObject<List<WeatherData>>(model);

                return View(data);
            }

            throw new Exception("Unable to get content");
        }
    }
}