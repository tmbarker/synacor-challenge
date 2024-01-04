namespace Synacor.Cli;

public delegate void CommandHandler(string arg);

public class Command(string name, string syntax, string desc, CommandHandler handler)
{
    public string Name { get; } = name;
    public string Syntax { get; } = syntax;
    public string Desc { get; } = desc;
    public CommandHandler Handler { get; } = handler;
}