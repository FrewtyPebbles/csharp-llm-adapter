using System.Threading.Channels;

namespace AIAgentAdapter.AgentBackends.BaseAgentBackend;

public abstract class BaseStreamResponse
{
    protected Channel<BaseResponseChunk> _chunkChannel = Channel.CreateUnbounded<BaseResponseChunk>();

    public IAsyncEnumerator<BaseResponseChunk> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        IAsyncEnumerator<BaseResponseChunk> chunkStream = _chunkChannel.Reader.ReadAllAsync(cancellationToken).GetAsyncEnumerator(cancellationToken);
        _ = Task.Run(async () => await ListenForChunks());
        return chunkStream;
    }

    public abstract Task ListenForChunks();
}