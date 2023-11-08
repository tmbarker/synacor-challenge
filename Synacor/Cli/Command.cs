namespace Synacor.Cli;

public delegate void CommandHandler(string arg);

public class Command
{
    public string Name { get; }
    public string Desc { get; }
    public CommandHandler Handler { get; }

    public Command(string name, string desc, CommandHandler handler)
    {
        Name = name;
        Desc = desc;
        Handler = handler;
    }
}