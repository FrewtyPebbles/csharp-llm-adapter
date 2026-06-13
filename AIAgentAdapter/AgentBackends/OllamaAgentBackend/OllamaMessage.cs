using System.Text.Json.Nodes;
using OllamaSharp.Models.Chat;
using System.Text;
using AIAgentAdapter.AgentBackends.BaseAgentBackend;

namespace AIAgentAdapter.AgentBackends.OllamaAgentBackend;

public record OllamaMessage(
        string Content, 
        MessageSender Sender, 
        List<ToolCallChunk>? ToolCalls = null, 
        List<byte[]>? Images = null, 
        string? Thinking = null
    ) : HistoryArtifact<Message>.BaseMessage(
        Content, Sender, ToolCalls, Images, Thinking
    )
{
    public override Message Serialize()
    {
        return new Message(
            role: Sender switch
            {
                MessageSender.Agent => ChatRole.Assistant,
                MessageSender.User => ChatRole.User,
                _ => throw new NotImplementedException()
            },
            content:Content,
            images:Images
                .Select(bytes => Encoding.UTF8.GetString(bytes))
                .ToArray()
        );
    }
}