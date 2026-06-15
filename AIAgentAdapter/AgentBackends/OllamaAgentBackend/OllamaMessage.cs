using System.Text.Json.Nodes;
using OllamaSharp.Models.Chat;
using System.Text;
using AIAgentAdapter.AgentBackends.BaseAgentBackend;

namespace AIAgentAdapter.AgentBackends.OllamaAgentBackend;

public record OllamaMessage : BaseMessageArtifact
{
    public OllamaMessage(string Content, MessageSender Sender, List<ToolCallChunk>? ToolCalls = null, List<byte[]>? Images = null, string? Thinking = null) : base(Content, Sender, ToolCalls, Images, Thinking)
    {
    }

    public override Message Serialize()
    {
        return new Message {
            Role = Sender switch
            {
                MessageSender.Agent => ChatRole.Assistant,
                MessageSender.User => ChatRole.User,
                _ => throw new NotImplementedException()
            },
            Content = Content,
            Images = [.. Images.Select(bytes => Encoding.UTF8.GetString(bytes))]
        };
    }
}