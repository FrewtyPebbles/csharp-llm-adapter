
using System.Text.Json.Nodes;

namespace AIAgentAdapter.AgentBackends.BaseAgentBackend;

public enum MessageSender
{
    Agent = 0,
    User = 1,
}

public abstract record HistoryArtifact<TSerialized>
{
    private HistoryArtifact() { }

    public abstract TSerialized Serialize();

    public abstract record BaseMessage(
        string Content, 
        MessageSender Sender, 
        List<ToolCallChunk>? ToolCalls = null, 
        List<byte[]>? Images = null, 
        string? Thinking = null
    ) : HistoryArtifact<TSerialized>
    {
        public List<ToolCallChunk> ToolCalls { get; init; } = ToolCalls ?? [];
        public List<byte[]> Images { get; init; } = Images ?? [];
    }

    public abstract record ToolResponse(
        string FunctionName, 
        string Response, 
        string? ID = null
    ) : HistoryArtifact<TSerialized>;
}