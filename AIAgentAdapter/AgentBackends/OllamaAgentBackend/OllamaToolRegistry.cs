using System;
using System.Collections.Generic;
using System.Text.Json;
using AIAgentAdapter.AgentBackends.BaseAgentBackend;

namespace AIAgentAdapter.AgentBackends.OllamaAgentBackend;

// Safe fallback for null tool lists passed to the base class
public class OllamaToolRegistry(List<Tool>? toolsList = null) : BaseToolRegistry(toolsList ?? [])
{
    // C# Convention: Public properties use PascalCase and initialize to prevent null reference bugs
    public List<OllamaSharp.Models.Chat.Tool> OllamaTools { get; private set; } = [];

    public override void RegisterTools()
    {
        var manualTools = ExportToOllamaTools();
        string serializedTools = JsonSerializer.Serialize(manualTools);
        
        // Coalesce to an empty list if the deserializer returns null
        OllamaTools = JsonSerializer.Deserialize<List<OllamaSharp.Models.Chat.Tool>>(serializedTools) ?? [];
    }

    /// <summary>
    /// Converts all registered C# tools into an object structured for the Ollama tools API payload.
    /// </summary>
    public List<object> ExportToOllamaTools()
    {
        var ollamaToolsList = new List<object>();

        foreach (var tool in Tools.Values)
        {
            var properties = new Dictionary<string, object>();
            var requiredArguments = new List<string>();

            foreach (var arg in tool.Arguments.Values)
            {
                properties[arg.Name] = new
                {
                    type = MapTypeToOllamaJson(arg.ArgType),
                    description = arg.Description
                };

                if (arg.Required)
                {
                    requiredArguments.Add(arg.Name);
                }
            }

            var toolDefinition = new
            {
                type = "function",
                function = new
                {
                    name = tool.Name,
                    description = tool.Description,
                    parameters = new
                    {
                        type = "object",
                        properties = properties,
                        required = requiredArguments
                    }
                }
            };

            ollamaToolsList.Add(toolDefinition);
        }

        return ollamaToolsList;
    }

    // Precise mapping matching JSON Schema spec standards for Ollama
    private static string MapTypeToOllamaJson(Type type)
    {
        if (type == typeof(int) || type == typeof(long) || type == typeof(short)) return "integer";
        if (type == typeof(double) || type == typeof(float) || type == typeof(decimal)) return "number";
        if (type == typeof(bool)) return "boolean";
        return "string"; 
    }
}
