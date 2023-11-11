namespace Synacor.Cli;

public delegate void CommandHandler(string arg);

public class Command
{
    public string Name { get; }
    public string Syntax { get; }
    public string Desc { get; }
    public CommandHandler Handler { get; }

    public Command(string name, string syntax, string desc, CommandHandler handler)
    {
        Name = name;
        Syntax = syntax;
        Desc = desc;
        Handler = handler;
    }
}