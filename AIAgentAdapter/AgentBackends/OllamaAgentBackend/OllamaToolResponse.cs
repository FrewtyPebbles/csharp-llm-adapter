using AIAgentAdapter.AgentBackends.BaseAgentBackend;
using OllamaSharp.Models.Chat;

namespace AIAgentAdapter.AgentBackends.OllamaAgentBackend;

public record OllamaToolResponse(
        string FunctionName, 
        string Response, 
        string? ID = null
    ) : HistoryArtifact<Message>.ToolResponse(FunctionName, Response, ID)
{
    public override Message Serialize()
    {
        return new Message
        {
            Role = ChatRole.Tool,
            Content = Response,
        };
    }
}