
/// <summary>
/// Event args
/// </summary>
public class StartAgentEventArgs
{
    /// <summary>
    /// zkopirovanej enum z adpointu, do budoucna by se mohly pridavat dalsi agenti
    /// odvazlivci by si tady mohli nareferencovat SangBO a ten enum si vzit odtamtud
    /// </summary>
    public enum Agents
    {
        eDispatchAgent
    }

    public Agents Agent { get; set; }
}
