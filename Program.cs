using Ocelot.DependencyInjection;
using Steeltoe.Discovery.Client;
using Steeltoe.Extensions.Configuration;
using System.Configuration;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using WebApp1.Controllers;
using System.Reflection;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging.Configuration;

var builder = WebApplication.CreateBuilder(args);

ConfigureLogs();
builder.Host.UseSerilog();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Services.AddOcelot();
builder.Services.AddDiscoveryClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(600);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthorization();

app.UseSession();

app.MapControllers();

app.Run();

void ConfigureLogs()
{
	{
		var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
		var configuration = new ConfigurationBuilder()
		    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
		    .AddJsonFile(
			   $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
			   optional: true)
		    .Build();

		Log.Logger = new LoggerConfiguration()
		    .Enrich.FromLogContext()
		    .Enrich.WithExceptionDetails()
		    .WriteTo.Debug()
		    .WriteTo.Console()
		    .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment))
		    .Enrich.WithProperty("Environment", environment)
		    .ReadFrom.Configuration(configuration)
		    .CreateLogger();
	}

	
}
ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
{
	return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
	{
		AutoRegisterTemplate = true,
		IndexFormat =  $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
	};
}