using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Channels;
using AIAgentAdapter.AgentBackends.BaseAgentBackend;
using OllamaSharp.Models.Chat;
using System.Linq;

namespace AIAgentAdapter.AgentBackends.OllamaAgentBackend;

public record ToolCallData(string Id, string  Name, IDictionary<string, object> ToolArguments);


public class OllamaStreamResponse(IAsyncEnumerable<ChatResponseStream?> rawStreamingResponse, OllamaAgent agent) : BaseStreamResponse
{
    public IAsyncEnumerable<ChatResponseStream?> _rawStreamingResponse = rawStreamingResponse;
    public OllamaAgent _agent = agent;

    record ToolTask(Task<string> Task, string ToolName, Dictionary<string, object> DictionaryArgs, string? ToolCallId);

    public async override Task ListenForChunks()
    {
        string? thinking = null;
        string content = "";
        string contentChunk;
        Message? message;

        List<ToolCallChunk> toolCalls = [];
        List<OllamaToolResponse> toolResponses = [];
        List<ToolTask> toolTasks = [];

        string toolResponseString;

        await foreach (var rawResponse in _rawStreamingResponse)
        {
            message = rawResponse?.Message;

            

            if (message?.Thinking is not null )
            {
                thinking ??= "";

                thinking += message?.Thinking;

                // Write to queue
                await _chunkChannel.Writer.WriteAsync(new ThinkingTokensChunk(message?.Thinking));
                
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
                foreach (Message.ToolCall toolCall in message.ToolCalls)
                {
                    if (toolCall.Function is not null && toolCall.Function.Name is not null)
                    {  
                        
                        string toolName = toolCall.Function.Name;

                        var dictionaryArgs = toolCall.Function?.Arguments?.ToDictionary<KeyValuePair<string, object>, string, object>(
                            kvp => kvp.Key,
                            kvp => kvp.Value
                        ) ?? new Dictionary<string, object>();

                        var toolCallChunk = new ToolCallChunk(toolName, dictionaryArgs, toolCall.Id);

                        toolCalls.Add(toolCallChunk);

                        await _chunkChannel.Writer.WriteAsync(toolCallChunk);

                        if (_agent._tools.Contains(toolName))
                        {
                            toolTasks.Add(new ToolTask(Task.Run(() => _agent._tools[toolName].Execute(dictionaryArgs)), toolName, dictionaryArgs, toolCall.Id));
                        }
                        else
                        {
                            OllamaToolResponse toolResponse = new(toolName, dictionaryArgs, $"The tool \"{toolName}\" is not an existing tool.", toolCall.Id);
                            toolResponses.Add(toolResponse);
                            await _chunkChannel.Writer.WriteAsync(new OllamaToolResponseChunk(toolResponse));
                        }
                    }
                }
            }

            if (rawResponse is { Done: true })
            {
                break;
            }
        }


        _agent._history.Add(new OllamaMessage(content, MessageSender.Agent, toolCalls, null, thinking));

        foreach (var toolTask in toolTasks)
        {
            OllamaToolResponse toolResponse = new(toolTask.ToolName, toolTask.DictionaryArgs, await toolTask.Task, toolTask.ToolCallId);
                toolResponses.Add(toolResponse);
                await _chunkChannel.Writer.WriteAsync(new OllamaToolResponseChunk(toolResponse));
        }

        foreach (OllamaToolResponse toolResponse in toolResponses)
        {
            _agent._history.Add(toolResponse);
        }

        _chunkChannel.Writer.Complete();
    }

    static object GetRawValue(JsonNode? node)
    {
        if (node is JsonValue jsonValue)
        {
            // Automatically unwrap primitives (bool, string, int, double, etc)
            if (jsonValue.TryGetValue(out string? s)) return s;
            if (jsonValue.TryGetValue(out double d)) return d;
            if (jsonValue.TryGetValue(out bool b)) return b;
            if (jsonValue.TryGetValue(out int i)) return i;
        }
        return node?.ToString() ?? "";
    }
}

