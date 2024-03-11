
using DOTNET_CRAWLER_AWS.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var mongoClient = new MongoClient("mongodb+srv://root:root@temporal.geu4dnl.mongodb.net/?retryWrites=true&w=majority&appName=temporal");
var database = mongoClient.GetDatabase("temporal");
var collection = database.GetCollection<WheaterModel>("WheaterModel");

builder.Services.AddSingleton(database);
builder.Services.AddTransient(_ => collection);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
