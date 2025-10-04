
using System.Runtime.CompilerServices;

class MockedStorytellingGenerator : IStorytellingGenerator
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public async IAsyncEnumerable<string> GenerateStoryAsync(string prompt, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Simulate generating a story in parts
        List<string> storyParts = new List<string>
        {
            "Once upon a time, in a land far away,",
            "there lived a brave knight named Sir Codealot.",
            "He embarked on a quest to find the legendary Bug-Free Castle.",
            "Along the way, he faced many challenges and made new friends.",
            "In the end, Sir Codealot found the castle and lived happily ever after."
        };

        foreach (string part in storyParts)
        {
            // Simulate some delay for generating each part
            Task.Delay(1000, cancellationToken).Wait(cancellationToken);
            yield return part;
        }
    }
}