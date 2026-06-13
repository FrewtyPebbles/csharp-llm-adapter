
using System.Text.Json.Nodes;

namespace AIAgentAdapter.AgentBackends.BaseAgentBackend;

public enum ResponseChunkType
{
    ResponseTokens,
    ThinkingTokens,
    ToolCall,
    ToolResponse,
}

public record BaseResponseChunk(ResponseChunkType Type);

public record ToolResponseChunk<THistoryArtifactSerialized>(HistoryArtifact<THistoryArtifactSerialized>.ToolResponse ToolResponse) : BaseResponseChunk(ResponseChunkType.ToolResponse);

public record ToolCallChunk(string FunctionName, JsonObject Arguments, string? ID = null) : BaseResponseChunk(ResponseChunkType.ToolCall);

public record ResponseTokensChunk(string Content, List<ToolCallChunk>? ToolCalls = null) : BaseResponseChunk(ResponseChunkType.ResponseTokens)
{
    public List<ToolCallChunk> ToolCalls { get; init; } = ToolCalls ?? [];
}

public record ThinkingTokensChunk(string Content, List<ToolCallChunk>? ToolCalls = null) : BaseResponseChunk(ResponseChunkType.ThinkingTokens)
{
    public List<ToolCallChunk> ToolCalls { get; init; } = ToolCalls ?? [];
}
