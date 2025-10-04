interface IStorytellingGenerator : IDisposable
{
    IAsyncEnumerable<string> GenerateStoryAsync(string prompt, CancellationToken cancellationToken = default);
}