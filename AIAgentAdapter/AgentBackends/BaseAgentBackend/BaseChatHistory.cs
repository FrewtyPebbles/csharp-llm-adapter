
using System.Text.Json.Nodes;

namespace AIAgentAdapter.AgentBackends.BaseAgentBackend;

public abstract record BaseChatHistory<TSerializedHistory, THistoryArtifactSerialized>(string SystemPrompt = "", List<HistoryArtifact<THistoryArtifactSerialized>>? History = null)
{

    public List<HistoryArtifact<THistoryArtifactSerialized>> History { get; init; } = History ?? [];

    public void Append(HistoryArtifact<THistoryArtifactSerialized> artifact)
    {
        History.Append(artifact);
    }

    public abstract TSerializedHistory Serialize();

}