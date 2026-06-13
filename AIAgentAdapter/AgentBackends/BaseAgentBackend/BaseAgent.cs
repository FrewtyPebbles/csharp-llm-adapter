namespace AIAgentAdapter.AgentBackends.BaseAgentBackend;

public abstract class BaseAgent(List<Tool>? tools = null)
{

    public abstract Task<BaseStreamResponse> Prompt(string content, List<byte[]>? images = null, bool think = true);
}