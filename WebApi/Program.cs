using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IStorytellingGenerator, AiStorytellingGenerator>();
var app = builder.Build();
app.MapPost("/", GetStoryAsync);
app.Run();

static async IAsyncEnumerable<string> GetStoryAsync(
    HttpContext httpContext,
    [FromBody] UserRequest userRequest,
    IStorytellingGenerator storyteller,
    [EnumeratorCancellation] CancellationToken cancellationToken
)
{
    // httpContext.Response.Headers.CacheControl = "no-cache";
    // httpContext.Response.Headers.Connection = "keep-alive";
    // httpContext.Response.Headers.ContentType = "text/event-stream";
    await foreach (string token in storyteller.GenerateStoryAsync("Tell me a story about a brave knight.", cancellationToken))
    {
        yield return token;
    }
}