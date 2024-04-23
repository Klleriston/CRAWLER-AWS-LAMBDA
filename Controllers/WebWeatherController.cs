using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DOTNET_CRAWLER_AWS.Models;

namespace DOTNET_CRAWLER_AWS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebWeatherController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public WebWeatherController(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        [HttpGet("weather/{city}")]
        public async Task<IActionResult> GetWeatherByCity(string city)
        {
            try
            {
                string apiKey = "YOUR_API_KEY";
                string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}";

                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Erro ao buscar dados meteorológicos.");
                }

                string content = await response.Content.ReadAsStringAsync();
                WeatherModel weatherData = JsonConvert.DeserializeObject<WeatherData>(content);

                double temperatureKelvin = weatherData.Main.Temp;
                double temperatureCelsius = temperatureKelvin - 273.15;

                var weatherModel = new WeatherModel
                {
                    CityName = weatherData.Name,
                    CurrentTemperature = temperatureCelsius
                };

                return Ok(weatherModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpGet("forecast/{city}")]
        public async Task<IActionResult> GetWeatherForecast(string city, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                string apiKey = "YOUR_API_KEY";
                string url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={apiKey}";

                if (startDate != null && endDate != null)
                {
                    url += $"&start={startDate:yyyy-MM-dd}&end={endDate:yyyy-MM-dd}";
                }

                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Erro ao buscar previsão do tempo.");
                }

                string content = await response.Content.ReadAsStringAsync();
                ForecastData forecastData = JsonConvert.DeserializeObject<ForecastData>(content);

                List<WeatherModel> weatherList = new List<WeatherModel>();

                foreach (var item in forecastData.List)
                {
                    double temperatureKelvin = item.Main.Temp;
                    double feelsLikeKelvin = item.Main.FeelsLike;

                    var weatherForecast = new WeatherModel
                    {
                        Date = item.DateTime,
                        Temperature = temperatureKelvin - 273.15,
                        FeelsLike = feelsLikeKelvin - 273.15
                    };

                    weatherList.Add(weatherForecast);
                }

                var forecastResult = new WeatherForecastResult
                {
                    Message = "Success",
                    City = new CityWeatherForecast
                    {
                        Name = city,
                        Weather = weatherList
                    }
                };

                return Ok(forecastResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }
    }
}
