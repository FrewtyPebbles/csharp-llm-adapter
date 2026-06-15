
using System.Text.Json.Nodes;

namespace AIAgentAdapter.AgentBackends.BaseAgentBackend;

public abstract record BaseChatHistory(string SystemPrompt = "", List<HistoryArtifact>? History = null)
{

    public List<HistoryArtifact> History { get; init; } = History ?? [];

    public void Add(HistoryArtifact artifact)
    {
        History.Add(artifact);
    }

    public abstract object Serialize();

}