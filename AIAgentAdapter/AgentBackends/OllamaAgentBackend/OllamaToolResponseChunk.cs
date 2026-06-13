using AIAgentAdapter.AgentBackends.BaseAgentBackend;
using OllamaSharp.Models.Chat;

namespace AIAgentAdapter.AgentBackends.OllamaAgentBackend;

public record OllamaToolResponseChunk(OllamaToolResponse OllamaToolResponse) : ToolResponseChunk<Message>(OllamaToolResponse);
