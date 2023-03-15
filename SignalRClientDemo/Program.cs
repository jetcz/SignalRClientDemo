using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddJsonFile("appSettings.json")
    .Build();

var adpointInstances = config.GetSection("instances").Get<AdpointInstance[]>();

foreach (var adpointInstance in adpointInstances)
{
    var client = new AgentsHubClient(adpointInstance);

    client.StartAgentRequest += Client_StartAgentRequest;
}

while (Console.ReadLine() != "exit") ;


//this method will be executed when we receive a signalr message
void Client_StartAgentRequest(object source, StartAgentEventArgs e)
{
    Console.WriteLine($"{DateTime.Now} {e}");

    //here we start the agent

    //need to handle some throttling in case we receive like 1000 messages per second, do not even attempt to start the agent, lets say that 1 message per second will be the limit
    //if we are able to go pass the throttling, then parallel runing of the agent should be prevented further down the line, probably by using named mutex
}
