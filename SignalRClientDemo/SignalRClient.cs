using Microsoft.AspNet.SignalR.Client;
using System.Net;

public class SignalRClient
{
    /// <summary>
    /// Declaration of a public event
    /// </summary>
    public event EventHandler? StartAgentRequest;

    /// <summary>
    /// Handler delegate
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    public delegate void EventHandler(object source, StartAgentEventArgs e);

    /// <summary>
    /// Calling event handler of "StartAgentRequest" event
    /// </summary>
    /// <param name="e"></param>
    protected virtual void StartAgentRequestHandler(StartAgentEventArgs args)
    {
        StartAgentRequest?.Invoke(this, args);
    }

    private readonly AdpointInstance _Instance;


    public SignalRClient(AdpointInstance instance)
    {
        _Instance = instance;

        Connect();
    }

    private void Connect()
    {
        var connection = new HubConnection(_Instance.URL, false) //adpoint web.config: AdpointSignalRURL
        {
            Credentials = CredentialCache.DefaultCredentials //ntlm, only useful for devs but it works with anonymous too
        };

        connection.Headers["X-Adpoint-SignalRKey"] = "supersecretkey"; //adpoint web.config: SignalRKey

        connection.ConnectionSlow += OnConnectionSlow;

        var hubProxy = connection.CreateHubProxy("agentsHub");

        hubProxy.On<string>("SendMessage", message => StartAgentRequestHandler(new StartAgentEventArgs() { Instance = _Instance.Name, AgentName = message })); //listen for messages

        var task = connection.Start();

        task.ContinueWith(_ =>
        {
            Console.WriteLine($"Connected to {connection.Url}");
        });

        task.ContinueWith(excHandler =>
        {
            Console.WriteLine($"Failed to connect {excHandler.Exception}");

        }, TaskContinuationOptions.OnlyOnFaulted);

    }

    private void OnConnectionSlow()
    {
        Console.WriteLine($"Connection slow");
    }
}