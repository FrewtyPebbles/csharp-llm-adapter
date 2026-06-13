using System;
using System.Collections.Generic;
using System.Linq;

namespace AIAgentAdapter.AgentBackends.BaseAgentBackend;

public enum ToolOrigin
{
    Remote,
    Client,
    Hybrid
}

public class Tool(
    string name,
    List<ToolArgument> arguments,
    string description,
    string returns,
    Func<Dictionary<string, object>, string> handlerCallback,
    ToolOrigin origin)
{
    // Public properties (equivalent to Python's self.X variables)
    public string Description { get; set; } = description;
    public string Name { get; set; } = name;
    public Dictionary<string, ToolArgument> Arguments { get; set; } = arguments.ToDictionary(arg => arg.Name, arg => arg);
    public string Returns { get; set; } = returns;
    public ToolOrigin Origin { get; set; } = origin;

    // The callback accepts a dictionary (kwargs) and returns an object
    public Func<Dictionary<string, object>, string> HandlerCallback { get; set; } = handlerCallback;
    public BaseToolRegistry? ToolRegistry { get; set; } = null;

    public string Execute(Dictionary<string, object> kwargs)
    {
        var failureList = new List<string>();

        foreach (var arg in Arguments.Values)
        {
            if (!kwargs.ContainsKey(arg.Name))
            {
                if (arg.Required)
                {
                    failureList.Add($"\"{arg.Name}\" is a required argument for the \"{Name}\" tool.");
                }
                else if (arg is not null && arg.DefaultValue is not null)
                {
                    kwargs[arg.Name] = arg.DefaultValue;
                }
                continue;
            }

            bool isValid = arg.Validate(kwargs[arg.Name]);
            if (!isValid && arg is not null && arg.ValidationFailMessage is not null)
            {
                failureList.Add(arg.ValidationFailMessage);
            }
        }

        if (failureList.Count > 0)
        {
            return $"Executing the \"{Name}\" tool failed for the following reasons: {string.Join(" ", failureList)}";
        }

        return HandlerCallback(kwargs);
    }

    public override string ToString()
    {
        string argumentsString = string.Join("\n", Arguments.Keys);
        
        return $"{Description}\n\n" +
               $"This tool executes on the {Origin switch {
                    ToolOrigin.Remote => "remote server",
                    ToolOrigin.Client => "client",
                    ToolOrigin.Hybrid => "client and remote server",
               }}.\n\n" +
               $"Args:\n{argumentsString}\n\n" +
               $"Returns:\n    {Returns}\n";
    }
}
