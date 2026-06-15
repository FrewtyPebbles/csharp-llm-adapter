using AIAgentAdapter.AgentBackends.BaseAgentBackend;
using OllamaSharp.Models.Chat;

namespace AIAgentAdapter.AgentBackends.OllamaAgentBackend;

public record OllamaToolResponse : ToolResponseArtifact
{
    public OllamaToolResponse(string FunctionName, Dictionary<string, object> Arguments, string Response, string? ID = null) : base(FunctionName, Arguments, Response, ID)
    {
    }

    public override Message Serialize()
    {
        return new Message
        {
            Role = ChatRole.Tool,
            ToolName = FunctionName,
            Content = Response,
        };
    }
}