using Microsoft.AspNetCore.Mvc;
using RestSharp;
using WebApp1.Models;

namespace WebApp1.Controllers
{
   [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet("getMovieById")]
        public IActionResult Get(long id)
        {
            var token = HttpContext.Session.GetString("token");
            var client = new RestClient();
            var request = new RestRequest($"http://movies-service:8080/home/movie/{id}", Method.Get);
            request.AddHeader("token", token);
            _logger.LogInformation("requesting movie");
            var response = client.Execute(request);
            return Ok(response.Content);
        }

        [HttpPost("postMovie")]
        public IActionResult Post(Movie movie)
        {
            var token = HttpContext.Session.GetString("token");
            var client = new RestClient();
            var request = new RestRequest($"http://movies-service:8080/home/addMovie/{movie.Id}", Method.Post);
            request.AddHeader("token", token);
            request.AddJsonBody(movie);
            _logger.LogInformation("inserting movie");
            var response = client.Execute(request);
            return Ok(response.StatusCode);
        }
    }
}
