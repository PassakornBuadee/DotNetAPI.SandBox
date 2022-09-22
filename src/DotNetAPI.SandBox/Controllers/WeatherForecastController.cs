using DotNetAPI.SandBox.DBContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DotNetAPI.SandBox.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        AppSettings _appSettings;
        private readonly SandBoxDbContext? _sandBoxDbContext;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, 
            AppSettings appSettings,
            SandBoxDbContext? sandBoxDbContext
            )
        {
            _logger = logger;
            _appSettings = appSettings;
            if(sandBoxDbContext == null) throw new ArgumentNullException(nameof(sandBoxDbContext));
            _sandBoxDbContext = sandBoxDbContext;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IActionResult> Get()
        {
            //see if data change according to appsettings | connection string
            //during debug /change property in appsettings.{env}.json

            //var settingResult = System.Text.Json.JsonSerializer.Serialize(_appSettings);
            //return Ok(settingResult);
            var user = await _sandBoxDbContext.User.FirstOrDefaultAsync(m => m.Username == "test1");
            if (user == null) throw new NullReferenceException();
            
            return Ok(_appSettings.ConnectionStrings.SandBoxConnectionString);
            //return Ok(user);
        }
    }
}