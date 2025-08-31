using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Polly;
using System.Text.Json;
using Webjet.Api.Constants;
using Webjet.Api.Hubs;
using Webjet.Api.Models.Options;
using Webjet.Api.Services;
using Webjet.Api.Services.Providers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("dev", p =>
        p.WithOrigins("https://localhost:5173", "http://localhost:5173")
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials());
});

builder.Services.AddSignalR()
    .AddJsonProtocol(o => o.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

builder.Services.AddMemoryCache();
builder.Services.AddOptions<WebjetSettings>().BindConfiguration("WebjetSettings");
builder.Services.AddOptions<CacheSettings>().BindConfiguration("Cache");

builder.Services.AddHttpClient(ProviderNames.Cinemaworld, (sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<WebjetSettings>>().Value;
    client.BaseAddress = new Uri(settings.CinemaworldBaseUrl.TrimEnd('/') + "/");
    client.DefaultRequestHeaders.Add("x-access-token", settings.ApiKey);
})
.AddResilienceHandler("cinemaworld-pipeline", ConfigureResiliencePipeline);

builder.Services.AddHttpClient(ProviderNames.Filmworld, (sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<WebjetSettings>>().Value;
    client.BaseAddress = new Uri(settings.FilmworldBaseUrl.TrimEnd('/') + "/");
    client.DefaultRequestHeaders.Add("x-access-token", settings.ApiKey);
})
.AddResilienceHandler("filmworld-pipeline", ConfigureResiliencePipeline);

builder.Services.AddSingleton<IMovieProvider>(sp =>
    new CinemaworldService(
        sp.GetRequiredService<IHttpClientFactory>(),
        sp.GetRequiredService<IOptions<WebjetSettings>>()));

builder.Services.AddSingleton<IMovieProvider>(sp =>
    new FilmworldService(
        sp.GetRequiredService<IHttpClientFactory>(),
        sp.GetRequiredService<IOptions<WebjetSettings>>()));

builder.Services.AddSingleton<IMovieAggregatorService, MoviesAggregatorService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("dev");
}

app.MapControllers();

app.MapHub<MoviesHub>("/hubs/movies");

app.Run();

static void ConfigureResiliencePipeline(ResiliencePipelineBuilder<HttpResponseMessage> pipeline)
{
    pipeline.AddRetry(new HttpRetryStrategyOptions
    {
        MaxRetryAttempts = 2,
        BackoffType = DelayBackoffType.Exponential,
        Delay = TimeSpan.FromMilliseconds(200),
        UseJitter = true
    });

    pipeline.AddTimeout(TimeSpan.FromSeconds(3));

    pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
    {
        SamplingDuration = TimeSpan.FromSeconds(30),
        MinimumThroughput = 10,
        FailureRatio = 0.5,
        BreakDuration = TimeSpan.FromSeconds(30)
    });
}
