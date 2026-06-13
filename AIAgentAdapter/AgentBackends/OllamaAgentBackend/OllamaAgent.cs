using System.Text.Json;
using System.Text.Json.Nodes;
using AIAgentAdapter.AgentBackends.BaseAgentBackend;
using OllamaSharp;
using OllamaSharp.Models.Chat;

namespace AIAgentAdapter.AgentBackends.OllamaAgentBackend;

public class OllamaAgent : BaseAgent
{

    OllamaApiClient _client;

    public string _model;

    public OllamaChatHistory _history;

    public OllamaToolRegistry _tools;

    public OllamaAgent(string Host, string Model, JsonObject? History = null, string SystemPrompt = "", List<BaseAgentBackend.Tool>? tools = null)
    {
        _client = new OllamaApiClient(new Uri(Host), Model);
        _model = Model;
        _tools = new(tools ?? []);
        
        if (History is not null && JsonSerializer.Deserialize<OllamaChatHistory>(History) is OllamaChatHistory deserializedChatHistory)
        {
            _history = deserializedChatHistory;
        }
        else
        {
            _history = new OllamaChatHistory(SystemPrompt);
        }
    }

    public override async Task<BaseStreamResponse> Prompt(string content, List<byte[]>? images = null, bool think = true)
    {
        images ??= [];

        OllamaMessage message = new(content, MessageSender.User, Images:images);
        _history.Add(message);
        IAsyncEnumerable<ChatResponseStream?> rawStreamingResponse = _client.ChatAsync(
            new ChatRequest
            {
                Model = _model,
                Messages = _history.Serialize(),
                Stream = true,
                Think = think,
                Tools = _tools.OllamaTools,
            }
        );
        return new OllamaStreamResponse(rawStreamingResponse, this);
    }
}