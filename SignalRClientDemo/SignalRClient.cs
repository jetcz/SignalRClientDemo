using Microsoft.AspNet.SignalR.Client;
using System.Net;

/// <summary>
/// SignalR client which connects to Adpoint
/// </summary>
public class AgentsHubClient
{
    public event EventHandler? StartAgentRequest;
    public delegate void EventHandler(object source, StartAgentEventArgs e);

    protected virtual void StartAgentRequestHandler(StartAgentEventArgs args)
    {
        StartAgentRequest?.Invoke(this, args);
    }

    private readonly AdpointInstance _Instance;
    private IHubProxy _HubProxy;
    private HubConnection _Connection;

    public AgentsHubClient(AdpointInstance instance)
    {
        _Instance = instance;

        Connect();
    }

    private void Connect()
    {
        _Connection = new HubConnection(_Instance.URL, false) //adpoint web.config: AdpointSignalRURL
        {
            Credentials = CredentialCache.DefaultCredentials //ntlm, only useful for devs but it works with anonymous too
        };

        _Connection.Headers["X-Adpoint-SignalRKey"] = "supersecretkey"; //adpoint web.config: SignalRKey

        _Connection.ConnectionSlow += OnConnectionSlow;
        _Connection.Reconnecting += OnReconnecting;
        _Connection.Reconnected += OnReconnected;
        _Connection.Closed += OnClosed;
        _Connection.Error += OnError;

        _HubProxy = _Connection.CreateHubProxy("agentsHub");

        _HubProxy.On<Message>("SendMessage", message => StartAgentRequestHandler(new StartAgentEventArgs() { Instance = _Instance.Name, Message = message })); //listen for messages and raise StartAgentRequest event on message received

        var task = _Connection.Start();

        task.ContinueWith(_ =>
        {
            Console.WriteLine($"{DateTime.Now} {_Instance.Name} Connected");
        });

        task.ContinueWith(excHandler =>
        {
            Console.WriteLine($"{DateTime.Now} {_Instance.Name} Failed to connect: {excHandler.Exception}");

            //try reconnect?

            //kill the agent runner if we are not connected to Adpoint?

        }, TaskContinuationOptions.OnlyOnFaulted);

    }

    private void OnError(Exception obj)
    {
        Console.WriteLine($"{DateTime.Now} {_Instance.Name} Error");        
    }

    private void OnClosed()
    {
        Console.WriteLine($"{DateTime.Now} {_Instance.Name} Closed");

        //when reconnecting, OnClosed will occur after 30s, meanwhile OnError is being raised few times
        //kill the agent runner when connection is closed?
    }

    private void OnReconnected()
    {
        Console.WriteLine($"{DateTime.Now} {_Instance.Name} Reconnected");

        //if the connection can be established again in 30s, OnReconnected is raised
    }

    private void OnReconnecting()
    {
        Console.WriteLine($"{DateTime.Now} {_Instance.Name} Reconnecting");

        //when the connection is closed (ie. Adpoint is killed), OnReconnecting is raised 
    }

    private void OnConnectionSlow()
    {
        //useless
        Console.WriteLine($"{DateTime.Now} {_Instance.Name} Connection slow");
    }
}