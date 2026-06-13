using System.Text.Json;
using System.Text.Json.Nodes;
using AIAgentAdapter.AgentBackends.BaseAgentBackend;
using OllamaSharp.Models.Chat;

namespace AIAgentAdapter.AgentBackends.OllamaAgentBackend;

public record OllamaChatHistory(string SystemPrompt = "", List<HistoryArtifact<Message>>? History = null) : BaseChatHistory<List<Message>, Message>(SystemPrompt, History)
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

        foreach (HistoryArtifact<Message> artifact in History)
        {
            serializedHistory.Add(artifact.Serialize());
        }

        return serializedHistory;
    }
}