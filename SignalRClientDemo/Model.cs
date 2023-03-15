
public record Message
{
    public string AgentName { get; set; } //we need to agree on some list of known agents

    public string Payload { get; set; } //Optional, currently unused
}

/// <summary>
/// Event args
/// </summary>
public record StartAgentEventArgs
{
    public string Instance { get; set; }

    public Message Message { get; set; }
}
