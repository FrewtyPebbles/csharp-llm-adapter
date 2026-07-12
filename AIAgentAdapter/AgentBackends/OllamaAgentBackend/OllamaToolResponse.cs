using AIAgentAdapter.AgentBackends.BaseAgentBackend;
using OllamaSharp.Models.Chat;

namespace AIAgentAdapter.AgentBackends.OllamaAgentBackend;

public record OllamaToolResponse(string FunctionName, Dictionary<string, object> Arguments, string Response, string? ID = null) 
    : ToolResponseArtifact(FunctionName, Arguments, Response, ID)
{
    public override Message Serialize()
    {
        return new()
        {
            Role = ChatRole.Tool,
            ToolName = FunctionName,
            Content = Response,
        };
    }
}