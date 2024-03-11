using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using DOTNET_CRAWLER_AWS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;


[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace DOTNET_CRAWLER_AWS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IMongoCollection<WheaterModel> _collection;

        public WeatherController()
        {
            
        }
        public WeatherController(HttpClient httpClient, IMongoCollection<WheaterModel> collection)
        {
            _httpClient = httpClient;
            _collection = collection;
        }

        [HttpGet]
        public async Task<IActionResult> GetAPI()
        {
            try
            {
                string url = "https://api.openweathermap.org/data/2.5/weather?q=liberdade&appid=3c118c14342b9baa4e9a8ea4ee8af0bc";
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

                await _collection.InsertOneAsync(weatherModel);

                var result = weatherModel;

                return Ok(weatherModel);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocorreu um erro: {ex.Message}");
            }
        }
    }
}
