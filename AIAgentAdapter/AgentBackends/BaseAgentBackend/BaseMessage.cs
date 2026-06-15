
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using AIAgentAdapter.AgentBackends.OllamaAgentBackend;

namespace AIAgentAdapter.AgentBackends.BaseAgentBackend;

public enum MessageSender
{
    Agent = 0,
    User = 1,
}

// OLLAMA DERIVED TYPES
[JsonDerivedType(typeof(OllamaMessage), typeDiscriminator: "Message")]
[JsonDerivedType(typeof(OllamaToolResponse), typeDiscriminator: "ToolResponse")]
// BASE CLASS
public abstract record HistoryArtifact
{
    protected HistoryArtifact() { }
    public abstract object Serialize();
}
public abstract record BaseMessageArtifact(
    string Content, 
    MessageSender Sender, 
    List<ToolCallChunk>? ToolCalls = null, 
    List<byte[]>? Images = null, 
    string? Thinking = null
) : HistoryArtifact
{
    public List<ToolCallChunk> ToolCalls { get; init; } = ToolCalls ?? [];
    public List<byte[]> Images { get; init; } = Images ?? [];
};

public abstract record ToolResponseArtifact(
    string FunctionName,
    Dictionary<string, object> Arguments,
    string Response,
    string? ID = null
) : HistoryArtifact;