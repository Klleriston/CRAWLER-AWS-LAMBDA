using DOTNET_CRAWLER_AWS.Data;
using DOTNET_CRAWLER_AWS.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DOTNET_CRAWLER_AWS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly WeatherRepository _weatherRepository;

        public WeatherController(HttpClient httpClient, WeatherRepository weatherRepository)
        {
            _httpClient = httpClient;
            _weatherRepository = weatherRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAPI()
        {
            try
            {
                string url = "https://api.openweathermap.org/data/2.5/weather?lat=-23.5505&lon=-46.6333&appid=f1cdb1003a7e05c670b29a5aabad3ce1";

                HttpResponseMessage res = await _httpClient.GetAsync(url);

                if (!res.IsSuccessStatusCode)
                {
                    return StatusCode((int)res.StatusCode, "Erro na requisição.");
                }

                string content = await res.Content.ReadAsStringAsync();
                dynamic resJSON = JsonConvert.DeserializeObject(content);

                double tempKelvin = (double)resJSON["main"]["temp"];
                double tempCelsius = tempKelvin - 273.15;

                string cityName = resJSON["name"].ToString();
                string currentTemp = tempCelsius.ToString("00");

                var weatherModel = new WheaterModel
                {
                    CityName = cityName,
                    CurrentTemperature = tempCelsius
                };

                await _weatherRepository.Insert(weatherModel);

                var result = new { CityName = cityName, CurrentTemp = currentTemp };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocorreu um erro: {ex.Message}");
            }
        }
    }
}
