namespace CommanderGQL.GraphQL.Commands;

public record UpdateCommandInput(int id, string HowTo, string CommandLine, int PlatformId);