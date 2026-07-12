namespace AIAgentAdapter.AgentBackends.BaseAgentBackend;

public abstract class BaseAgent(List<Tool>? tools = null)
{

    public abstract BaseStreamResponse Prompt(string content, List<byte[]>? images = null, bool think = true);
    public abstract IAsyncEnumerable<ModelInstallStatus> InstallModel(string model, CancellationToken cancellationToken = default);
}

public enum ModelInstallStatusState
{
    Installing,
    Finished
}

public record ModelInstallStatus(string Model, float Progress, ModelInstallStatusState InstallState, string InstallStatusMessage);