
using System.Runtime.CompilerServices;
using LLama;
using LLama.Common;
using LLama.Sampling;

class AiStorytellingGenerator : IStorytellingGenerator
{
    private const string modelPath = @"C:\\Users\\felic\\AppData\\Local\\llama.cpp\\unsloth_Qwen3-4B-GGUF_Qwen3-4B-Q8_0.gguf"; // change it to your own model path.
    private readonly LLamaWeights _model;
    private readonly LLamaContext _context;
    private readonly ModelParams _parameters;
    private readonly InferenceParams _inferenceParams;
    private readonly InteractiveExecutor _executor;

    public AiStorytellingGenerator()
    {
        _parameters = new ModelParams(modelPath)
        {
            ContextSize = 4096,
            Embeddings = true,
            GpuLayerCount = 36
        };
        _inferenceParams = new InferenceParams()
        {
            MaxTokens = 256,
            AntiPrompts = new List<string> { "User:" },
            SamplingPipeline = new DefaultSamplingPipeline()
            {
                Temperature = 0.8f,
            },
        };
        _model = LLamaWeights.LoadFromFile(_parameters);
        _context = _model.CreateContext(_parameters);
        _executor = new InteractiveExecutor(_context);
    }
    
    public async IAsyncEnumerable<string> GenerateStoryAsync(string prompt, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ChatHistory chatHistory = new ChatHistory();
        chatHistory.AddMessage(AuthorRole.System, "Assist the user in creating a story. Keep the story short with less than 200 words.");
        ChatSession session = new(_executor, chatHistory);
        await foreach (string token in session.ChatAsync(new ChatHistory.Message(AuthorRole.User, prompt), _inferenceParams, cancellationToken))
        {
            yield return token;
        }
    }

    public void Dispose()
    {
        _context?.Dispose();
        _model?.Dispose();
        GC.SuppressFinalize(this);
    }
}