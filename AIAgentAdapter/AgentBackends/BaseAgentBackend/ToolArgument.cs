namespace AIAgentAdapter.AgentBackends.BaseAgentBackend;

public class ToolArgument
{
    public string Name { get; set; }
    public Type ArgType { get; set; }
    public object? DefaultValue { get; set; }
    public string Description { get; set; }
    public bool Required { get; set; }
    
    public Predicate<object> Validator { get; set; }
    public string? ValidationFailMessage { get; set; }

    public ToolArgument(
        string name,
        Type argType,
        string description,
        object? defaultValue = null,
        bool required = true,
        Predicate<object>? validator = null,
        string? validationFailMessage = null)
    {
        Name = name;
        ArgType = argType;
        Description = description;
        DefaultValue = defaultValue;
        Required = required;
        
        Validator = validator ?? (val => true);
        ValidationFailMessage = validationFailMessage;
    }

    public bool Validate(object argumentValue)
    {
        return Validator(argumentValue);
    }

    public override string ToString()
    {
        string requiredText = Required ? "This argument is required." : "This argument is not required.";
        
        string defaultText = DefaultValue != null ? $"Defaults to {DefaultValue}. " : "";

        return $"    {Name} ({ArgType.Name}): {requiredText} {defaultText}{Description} ";
    }
}
