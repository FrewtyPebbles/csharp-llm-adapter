namespace AIAgentAdapter.AgentBackends.BaseAgentBackend;

public abstract class BaseAgent(List<Tool>? tools = null)
{

    public abstract BaseStreamResponse Prompt(string content, List<byte[]>? images = null, bool think = true);
}