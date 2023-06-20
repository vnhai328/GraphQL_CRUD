using CommanderGQL.Models;

namespace CommanderGQL.GrapQL;

public class Subscription
{
    [Subscribe]
    [Topic]
    public Platform OnPlatformAdded([EventMessage] Platform platform)
    {
        return platform;
    }

    [Subscribe]
    [Topic]
    public Command OnCommandAdded([EventMessage] Command command)
    {
        return command;
    }
}