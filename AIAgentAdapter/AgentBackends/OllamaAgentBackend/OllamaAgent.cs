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

    public OllamaAgent(string Host, string Model, string SystemPrompt, List<BaseAgentBackend.Tool>? tools = null)
    {
        _client = new OllamaApiClient(new Uri(Host), Model);
        _model = Model;
        _tools = new(tools ?? []);
        _history = new OllamaChatHistory(SystemPrompt);
    }

    public OllamaAgent(string Host, string Model, OllamaChatHistory History, List<BaseAgentBackend.Tool>? tools = null)
    {
        _client = new OllamaApiClient(new Uri(Host), Model);
        _model = Model;
        _tools = new(tools ?? []);
        _history = History;
    }

    public override BaseStreamResponse Prompt(string content, List<byte[]>? images = null, bool think = true)
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