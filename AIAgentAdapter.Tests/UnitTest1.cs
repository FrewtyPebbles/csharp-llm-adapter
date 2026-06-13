using AIAgentAdapter.AgentBackends.BaseAgentBackend;
using AIAgentAdapter.AgentBackends.OllamaAgentBackend;
using Xunit.Abstractions;
using Xunit;

namespace AIAgentAdapter.Tests;

public class UnitTest1
{

    private readonly ITestOutputHelper _output;

    // 👈 xUnit automatically injects this helper here
    public UnitTest1(ITestOutputHelper output)
    {
        _output = output;
    }

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
        Console.WriteLine("TEST FINISHED");
    }
}
