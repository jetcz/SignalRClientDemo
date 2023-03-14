internal class Program
{
    private static void Main(string[] args)
    {
        var client = new SignalRClient();

        client.StartAgentRequest += Client_StartAgentRequest;

        while (Console.ReadLine() != "exit") ;
    }

    private static void Client_StartAgentRequest(object source, StartAgentEventArgs e)
    {
        Console.WriteLine($"Start {e.Agent}");
    }
}
