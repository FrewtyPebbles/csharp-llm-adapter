using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Channels;
using AIAgentAdapter.AgentBackends.BaseAgentBackend;
using OllamaSharp.Models.Chat;

namespace AIAgentAdapter.AgentBackends.OllamaAgentBackend;

public record ToolCallData(string Id, string  Name, IDictionary<string, object> ToolArguments);


public class OllamaStreamResponse : BaseStreamResponse
{
    public IAsyncEnumerable<ChatResponseStream?> _rawStreamingResponse;
    public OllamaAgent _agent;
    public OllamaStreamResponse(IAsyncEnumerable<ChatResponseStream?> rawStreamingResponse, OllamaAgent agent)
    {
        _rawStreamingResponse = rawStreamingResponse;
        _agent = agent;
    }

    public async override Task ListenForChunks()
    {
        string? thinking = null;
        string content = "";
        string contentChunk;
        Message? message;

        var detectedToolCalls = new List<ToolCallData>();


        await foreach (var rawResponse in _rawStreamingResponse)
        {
            message = rawResponse?.Message;

            

            if (message?.Thinking is not null )
            {
                thinking ??= "";

                if (message.Content is not null)
                {
                    thinking += message.Content;

                    // Write to queue
                    await _chunkChannel.Writer.WriteAsync(new ThinkingTokensChunk(message.Content));
                }
                continue;
            }
            
            if (message?.Content is not null)
            {
                contentChunk = message.Content;
                content += contentChunk;
                await _chunkChannel.Writer.WriteAsync(new ResponseTokensChunk(message.Content));
            }

            if (message?.ToolCalls is not null && message.ToolCalls.Any())
            {
                foreach (var toolCall in message.ToolCalls)
                {
                    if (toolCall.Function is not null)
                    {  
                        ToolCallData toolCallData = new ToolCallData(toolCall.Id, toolCall.Function.Name, toolCall.Function.Arguments);
                        detectedToolCalls.Add(toolCallData);
                        
                        string toolName = toolCall.Function.Name;

                        var nodeDictionary = toolCallData.ToolArguments.ToDictionary<KeyValuePair<string, object>, string, JsonNode?>(
                            kvp => kvp.Key,
                            kvp => JsonValue.Create(kvp.Value)
                        );

                        var jsonObject = new JsonObject(nodeDictionary);

                        await _chunkChannel.Writer.WriteAsync(new ToolCallChunk(toolName, jsonObject, toolCallData.Id));
                    }
                }
            }

            if (rawResponse is { Done: true })
            {
                break;
            }
        }

        if (detectedToolCalls.Count != 0)
        {
            // TODO: Call the triggered tools
        }

        _chunkChannel.Writer.Complete();
    }
}