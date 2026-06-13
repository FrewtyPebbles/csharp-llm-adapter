using AIAgentAdapter.AgentBackends.BaseAgentBackend;
using AIAgentAdapter.AgentBackends.OllamaAgentBackend;

namespace AIAgentAdapter.Tests;

public class UnitTest1
{

    [Fact]
    public async void GenerateResponse()
    {
        var agent = new OllamaAgent("http://127.0.0.1:11434", "gemma4:e4b", SystemPrompt:"You are an AI assistant.");
        await foreach (var chunk in await agent.Prompt("What is your favorite color?"))
        {
            if (chunk.Type == ResponseChunkType.ResponseTokens && chunk is ResponseTokensChunk responseTokens)
            {
                Console.Write(responseTokens.Content);
            }
        }
    }
}
