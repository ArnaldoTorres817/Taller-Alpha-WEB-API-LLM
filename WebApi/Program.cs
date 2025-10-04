using Microsoft.AspNetCore.Mvc;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IStorytellingGenerator, AiStorytellingGenerator>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
WebApplication app = builder.Build();
app.UseCors("AllowAll");
app.MapPost("/", GetStoryAsync);
app.MapPost("/api/stream", GetStoryStreamedAsync);
app.Run();

static async Task GetStoryStreamedAsync(
    HttpContext httpContext,
    [FromBody] UserRequest userRequest,
    IStorytellingGenerator storyteller)
{
    CancellationToken cancellationToken = httpContext.RequestAborted;
    httpContext.Response.ContentType = "text/plain; charset=utf-8";
    httpContext.Response.Headers.CacheControl = "no-cache";

    await foreach (string token in storyteller.GenerateStoryAsync(userRequest.UserPrompt, httpContext.RequestAborted))
    {
        if (httpContext.RequestAborted.IsCancellationRequested) break;
        await httpContext.Response.WriteAsync(token, cancellationToken);
        await httpContext.Response.Body.FlushAsync();
    }
}

static async Task GetStoryAsync(
        HttpContext httpContext,
        [FromBody] UserRequest userRequest,
        IStorytellingGenerator storyteller)
{
    // --- STEP 1: Set Headers for Server-Sent Events (SSE) ---
    HttpResponse response = httpContext.Response;
    response.ContentType = "text/event-stream";
    response.Headers.CacheControl = "no-cache";
    response.Headers.Connection = "keep-alive";

    // Use the request's CancellationToken for cancellation
    CancellationToken cancellationToken = httpContext.RequestAborted;

    // --- STEP 2: Generate the story and stream the response ---
    try
    {
        // Use a StreamWriter for easier text writing
        await using StreamWriter writer = new StreamWriter(response.Body);

        // Your original story generation call
        await foreach (string token in storyteller.GenerateStoryAsync(userRequest.UserPrompt, cancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            // --- STEP 3: Write data in the SSE format ---
            // Format: data: [message]\n\n
            // The extra newline (\n\n) is crucial; it signals the end of an event.
            await writer.WriteLineAsync($"data: {token}");
            // await writer.WriteLineAsync(); // The second newline

            // --- STEP 4: Flush the stream ---
            // This ensures the data is immediately sent to the client.
            await writer.FlushAsync();
        }

        // Optional: Send a final event to signify completion or close the stream
        await writer.WriteLineAsync("event: end");
        // await writer.WriteLineAsync("data: COMPLETE");
        await writer.WriteLineAsync();
        await writer.FlushAsync();
    }
    catch (OperationCanceledException)
    {
        // This is expected if the client disconnects
        httpContext.RequestServices.GetRequiredService<ILogger<Program>>()
            .LogInformation("Client disconnected, stopping story streaming.");
    }
    catch (Exception ex)
    {
        // Log the exception
        httpContext.RequestServices.GetRequiredService<ILogger<Program>>()
            .LogError(ex, "An error occurred during story streaming.");
    }
}