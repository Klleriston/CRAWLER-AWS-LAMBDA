using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using Amazon.Lambda.APIGatewayEvents;
using DOTNET_CRAWLER_AWS.Models;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace DOTNET_CRAWLER_AWS
{
    public class WeatherFunction
    {
        private readonly HttpClient _httpClient;
        private readonly IMongoCollection<WheaterModel> _collection;

        public WeatherFunction()
        {
            _httpClient = new HttpClient();

            
            var mongoClient = new MongoClient("mongodb+srv://root:root@temporal.geu4dnl.mongodb.net/?retryWrites=true&w=majority&appName=temporal");
            var database = mongoClient.GetDatabase("temporal"); 
            _collection = database.GetCollection<WheaterModel>("WheaterModel"); 
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                string apiKey = "3c118c14342b9baa4e9a8ea4ee8af0bc";
                string city = "liberdade";
                string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}";

                HttpResponseMessage res = await _httpClient.GetAsync(url);

                if (!res.IsSuccessStatusCode)
                {
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = (int)res.StatusCode,
                        Body = "Erro na requisição."
                    };
                }

                string content = await res.Content.ReadAsStringAsync();
                dynamic resJSON = JsonConvert.DeserializeObject(content);

                double tempKelvin = (double)resJSON.main.temp;
                double tempCelsius = tempKelvin - 273.15;

                string cityName = resJSON.name;

                var weatherModel = new WheaterModel
                {
                    CityName = cityName,
                    CurrentTemperature = tempCelsius
                };

                await _collection.InsertOneAsync(weatherModel);

                var response = new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = JsonConvert.SerializeObject(weatherModel),
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };

                return response;
            }
            catch (Exception ex)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = $"Ocorreu um erro: {ex.Message}"
                };
            }
        }
    }
}
