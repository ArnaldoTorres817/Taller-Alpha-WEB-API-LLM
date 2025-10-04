
using System.Runtime.CompilerServices;
using LLama;
using LLama.Common;
using LLama.Sampling;

class AiStorytellingGenerator : IStorytellingGenerator
{
    private readonly string? _modelPath;
    private readonly LLamaWeights _model;
    private readonly LLamaContext _context;
    private readonly ModelParams _parameters;
    private readonly InferenceParams _inferenceParams;
    private readonly InteractiveExecutor _executor;

    public AiStorytellingGenerator(IConfiguration configuration)
    {
        _modelPath = configuration.GetConnectionString("ModelPath");
        if (string.IsNullOrEmpty(_modelPath))
        {
            throw new FileNotFoundException("Model file not found. Please check the ModelPath configuration.", _modelPath);
        }
        _parameters = new ModelParams(_modelPath)
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
        // chatHistory.AddMessage(AuthorRole.System, "Assist the user in creating a story. Keep the story short with less than 200 words.");
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