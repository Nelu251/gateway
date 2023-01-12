using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Drawing.Printing;
using WebApp1.Models;

namespace WebApp1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly ILogger _logger;
        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpPost("register")]
        public IActionResult Post([FromBody] User user)
        {
            _logger.LogInformation("inside register controller");
            var client = new RestClient();
            var request = new RestRequest("http://auth-service:8081/register/", Method.Post);
            request.AddJsonBody(user);
            _logger.LogInformation("creating new account");
            var response = client.Execute(request);
			var token = response.Content;
            Console.WriteLine(token);
            HttpContext.Session.SetString("token", token);

            return Ok(response.Content);
        }

        [HttpPost("login")]
        public IActionResult Login(User user)
        {
            _logger.LogInformation("inside login controller");
            var client = new RestClient();
            var request = new RestRequest("http://auth-service:8081/login/", Method.Post);
            request.AddJsonBody(user);
            _logger.LogInformation("signing in ...");
            var response = client.Execute(request);
            var token = response.Content;
            HttpContext.Session.SetString("token", token);

            return Ok(response.Content);
        }
    }
}
