using System.Text.Json;
using AIAgentAdapter.AgentBackends.BaseAgentBackend;
using AIAgentAdapter.AgentBackends.OllamaAgentBackend;

namespace AIAgentAdapter.Tests;

public class UnitTest1
{

    [Fact]
    public async void GenerateResponse()
    {
        Console.WriteLine(" ***** GenerateResponse");
        using var agent = new OllamaAgent("http://127.0.0.1:11434", "gemma4:e4b", SystemPrompt:"You are an AI assistant.");
        var thinking = false;
        await foreach (var chunk in agent.Prompt("What is your favorite color?"))
        {

            if (chunk.Type == ResponseChunkType.ThinkingTokens && chunk is ThinkingTokensChunk thinkingTokens)
            {
                if (!thinking)
                {
                    Console.WriteLine("\n\nTHINKING\n\n");
                    thinking = true;
                }
                
                Console.Write(thinkingTokens.Content);
            }

            if (chunk.Type == ResponseChunkType.ResponseTokens && chunk is ResponseTokensChunk responseTokens)
            {
                if (thinking)
                {
                    Console.WriteLine("\n\nFINISHED THINKING\n\n");
                    thinking = false;
                }
                
                Console.Write(responseTokens.Content);
            }

            if (chunk.Type == ResponseChunkType.ToolResponse && chunk is OllamaToolResponseChunk toolResponseChunk)
            {
                Console.Write(toolResponseChunk.OllamaToolResponse.Response);
            }
        }
        Console.WriteLine();
    }

    string Tool(Dictionary<string, object> arguments) {
        string keyValue = arguments["Key"]?.ToString() ?? "";
        var res = keyValue == "waffle" ? "astronaut" : "DENIED";
        return res;
    }

    [Fact]
    public async void GenerateResponseTool()
    {
        Console.WriteLine(" ***** GenerateResponseTool");
        using var agent = new OllamaAgent(
            "http://127.0.0.1:11434",
            "gemma4:e4b",
            "You are an AI assistant.",
            [
                new("GetPasscode", [
                        new("Key", typeof(string),
                            description: "The key is \"waffle\".  Use this key as the value for this parameter if you want the passcode.",
                            required: true
                        )
                    ],
                    "This tool gets you the passcode if you put in the right key.",
                    "It returns the passcode given the correct key.",
                    Tool,
                    ToolOrigin.Client
                ),
            ]
        );
        var thinking = false;
        await foreach (var chunk in agent.Prompt("What is the passcode? Use it in a sentence."))
        {

            if (chunk.Type == ResponseChunkType.ThinkingTokens && chunk is ThinkingTokensChunk thinkingTokens)
            {
                if (!thinking)
                {
                    Console.WriteLine("\n\nTHINKING\n\n");
                    thinking = true;
                }
                
                Console.Write(thinkingTokens.Content);
            }

            if (chunk.Type == ResponseChunkType.ResponseTokens && chunk is ResponseTokensChunk responseTokens)
            {
                if (thinking)
                {
                    Console.WriteLine("\n\nFINISHED THINKING\n\n");
                    thinking = false;
                }
                
                Console.Write(responseTokens.Content);
            }

            if (chunk.Type == ResponseChunkType.ToolResponse && chunk is OllamaToolResponseChunk toolResponseChunk)
            {
                Console.Write(toolResponseChunk.OllamaToolResponse.Response);
            }
        }

        Console.WriteLine();
    }

    [Fact]
    public async Task SerializeChatHistory()
    {
        Console.WriteLine(" ***** SerializeChatHistory");
        using var agent = new OllamaAgent(
            "http://127.0.0.1:11434",
            "gemma4:e4b",
            "You are an AI assistant.",
            [
                new("GetPasscode", [
                        new("Key", typeof(string),
                            description: "The key is \"waffle\".  Use this key as the value for this parameter if you want the passcode.",
                            required: true
                        )
                    ],
                    "This tool gets you the passcode if you put in the right key.",
                    "It returns the passcode given the correct key.",
                    Tool,
                    ToolOrigin.Client
                ),
            ]
        );

        var thinking = false;
        await foreach (var chunk in agent.Prompt("What is the passcode?"))
        {

            if (chunk.Type == ResponseChunkType.ThinkingTokens && chunk is ThinkingTokensChunk thinkingTokens)
            {
                if (!thinking)
                {
                    Console.WriteLine("\n\nTHINKING\n\n");
                    thinking = true;
                }
                
                Console.Write(thinkingTokens.Content);
            }

            if (chunk.Type == ResponseChunkType.ResponseTokens && chunk is ResponseTokensChunk responseTokens)
            {
                if (thinking)
                {
                    Console.WriteLine("\n\nFINISHED THINKING\n\n");
                    thinking = false;
                }
                
                Console.Write(responseTokens.Content);
            }

            if (chunk.Type == ResponseChunkType.ToolResponse && chunk is OllamaToolResponseChunk toolResponseChunk)
            {
                Console.Write(toolResponseChunk.OllamaToolResponse.Response);
            }
        }

        thinking = false;
        await foreach (var chunk in agent.Prompt("Have you used the GetPasscode tool already?  If so what was the passcode?"))
        {

            if (chunk.Type == ResponseChunkType.ThinkingTokens && chunk is ThinkingTokensChunk thinkingTokens)
            {
                if (!thinking)
                {
                    Console.WriteLine("\n\nTHINKING\n\n");
                    thinking = true;
                }
                
                Console.Write(thinkingTokens.Content);
            }

            if (chunk.Type == ResponseChunkType.ResponseTokens && chunk is ResponseTokensChunk responseTokens)
            {
                if (thinking)
                {
                    Console.WriteLine("\n\nFINISHED THINKING\n\n");
                    thinking = false;
                }
                
                Console.Write(responseTokens.Content);
            }

            if (chunk.Type == ResponseChunkType.ToolResponse && chunk is OllamaToolResponseChunk toolResponseChunk)
            {
                Console.Write(toolResponseChunk.OllamaToolResponse.Response);
            }
        }

        Console.WriteLine();

        
        Console.WriteLine(JsonSerializer.Serialize(agent._history.Serialize()));
    }
}
