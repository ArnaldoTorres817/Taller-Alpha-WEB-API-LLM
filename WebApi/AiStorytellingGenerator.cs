
using System.Runtime.CompilerServices;
using LLama;
using LLama.Common;
using LLama.Sampling;

class AiStorytellingGenerator : IStorytellingGenerator
{
    public async IAsyncEnumerable<string> GenerateStoryAsync(string prompt, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string modelPath = @"C:\\Users\\felic\\AppData\\Local\\llama.cpp\\unsloth_Qwen3-4B-GGUF_Qwen3-4B-Q8_0.gguf"; // change it to your own model path.

        var parameters = new ModelParams(modelPath)
        {
            ContextSize = 4096,
            Embeddings = true,
            GpuLayerCount = 36
        };
        using var model = LLamaWeights.LoadFromFile(parameters);
        using var context = model.CreateContext(parameters);
        var executor = new InteractiveExecutor(context);
        var chatHistory = new ChatHistory();
        chatHistory.AddMessage(AuthorRole.System, "Assist the user in creating a story. Keep the story short with less than 200 words.");
        // chatHistory.AddMessage(AuthorRole.User, prompt);
        ChatSession session = new(executor, chatHistory);
        InferenceParams inferenceParams = new InferenceParams()
        {
            MaxTokens = 256,
            AntiPrompts = new List<string> { "User:" },
            SamplingPipeline = new DefaultSamplingPipeline()
            {
                Temperature = 0.8f,
            },
        };
        await foreach(string token in session.ChatAsync(new ChatHistory.Message(AuthorRole.User, prompt), inferenceParams, cancellationToken))
        {
            yield return token;
        }
    }
}