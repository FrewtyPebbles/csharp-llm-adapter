using System.Text.Json;
using System.Text.Json.Nodes;
using AIAgentAdapter.AgentBackends.BaseAgentBackend;
using OllamaSharp.Models.Chat;

namespace AIAgentAdapter.AgentBackends.OllamaAgentBackend;

public record OllamaChatHistory(string SystemPrompt = "", List<HistoryArtifact>? History = null) : BaseChatHistory(SystemPrompt, History)
{
    public override List<Message> Serialize()
    {
        List<Message> serializedHistory = new List<Message>
        {
            new Message
            {
                Role = ChatRole.System,
                Content = SystemPrompt
            }
        };

        foreach (HistoryArtifact artifact in History)
        {
            serializedHistory.Add((Message)artifact.Serialize());
        }

        return serializedHistory;
    }
}