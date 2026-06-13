
using System.Text.Json.Nodes;

namespace AIAgentAdapter.AgentBackends.BaseAgentBackend;

public abstract record BaseChatHistory<TSerializedHistory, THistoryArtifactSerialized>(string SystemPrompt = "", List<HistoryArtifact<THistoryArtifactSerialized>>? History = null)
{

    public List<HistoryArtifact<THistoryArtifactSerialized>> History { get; init; } = History ?? [];

    public void Add(HistoryArtifact<THistoryArtifactSerialized> artifact)
    {
        History.Add(artifact);
    }

    public abstract TSerializedHistory Serialize();

}