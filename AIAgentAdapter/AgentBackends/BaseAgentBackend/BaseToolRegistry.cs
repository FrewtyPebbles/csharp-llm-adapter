
using System.Text.Json;
using OllamaSharp.Models;

namespace AIAgentAdapter.AgentBackends.BaseAgentBackend;

public abstract class BaseToolRegistry
{
    public Dictionary<string, Tool> Tools { get; private set; }

    public BaseToolRegistry(List<Tool>? toolsList = null)
    {
        Tools = [];

        if (toolsList != null)
        {
            foreach (var tool in toolsList)
            {
                tool.ToolRegistry = this;
                
                Tools[tool.Name] = tool;
            }
        }

        RegisterTools();
    }

    public bool Contains(string toolName)
    {
        return Tools.ContainsKey(toolName);
    }

    public Tool this[string key]
    {
        get
        {
            if (!Tools.ContainsKey(key))
            {
                throw new KeyNotFoundException($"The tool '{key}' was not found in the registry.");
            }
            return Tools[key];
        }
        set
        {
            Tools[key] = value;
        }
    }

    public abstract void RegisterTools();
    
}
