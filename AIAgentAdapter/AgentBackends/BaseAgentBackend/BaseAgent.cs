namespace AIAgentAdapter.AgentBackends.BaseAgentBackend;

public abstract class BaseAgent
{
    public abstract Task<BaseStreamResponse> Prompt(string content, List<byte[]>? images = null);
}