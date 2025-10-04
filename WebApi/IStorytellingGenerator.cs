interface IStorytellingGenerator
{
    IAsyncEnumerable<string> GenerateStoryAsync(string prompt, CancellationToken cancellationToken = default);
}