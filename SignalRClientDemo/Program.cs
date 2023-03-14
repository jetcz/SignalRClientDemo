using Microsoft.Extensions.Configuration;

internal class Program
{
    private static void Main(string[] args)
    {
        var config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json")
                .Build();

        var adpointInstances = config.GetSection("instances").Get<AdpointInstance[]>();

        foreach (var adpointInstance in adpointInstances)
        {
            var client = new SignalRClient(adpointInstance);

            client.StartAgentRequest += Client_StartAgentRequest;
        }

        while (Console.ReadLine() != "exit") ;
    }

    private static void Client_StartAgentRequest(object source, StartAgentEventArgs e)
    {
        Console.WriteLine($"Start {e.AgentName} agent on {e.Instance}");
    }
}