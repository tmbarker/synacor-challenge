using Synacor.Utilities;
using Synacor.VirtualMachine;

namespace Synacor.Cli;

/// <summary>
/// A shell for working with Synacor <see cref="Vm"/> instances
/// </summary>
public class Shell
{
    private const string StartMessage = $"{nameof(Synacor)} {nameof(Shell)} started.";
    private const string EndedMessage = $"{nameof(Synacor)} {nameof(Shell)} ended.";
    private const string HelpCmdId = "help";
    private const string QuitCmdId = "quit";
    private const string PassThruPrefix = "--";
    private const string HelpHint = $"- Type '{HelpCmdId}' for cmd listing";
    private const string QuitHint = $"- Type '{QuitCmdId}' to quit";
    private const string PassThruHint =
        $"- Commands prepended with '{PassThruPrefix}' are passed thru to the VM instance input buffer";
    
    private readonly Dictionary<string, Command> _commandTable;
    private readonly Context _context;
    
    public Shell()
    {
        _commandTable = BuildCommandTable();
        _context = new Context(vm: Vm.New());
    }

    public void Start()
    {
        Log(StartMessage);
        Log(HelpHint);
        Log(QuitHint);
        Log(PassThruHint);
        Log();
        
        while (true)
        {
            var input = Console.ReadLine() ?? string.Empty;
            var passThru = input.StartsWith(PassThruPrefix);

            if (input == QuitCmdId)
            {
                Log(EndedMessage);
                return;
            }
            
            if (passThru)
            {
                _context.Vm.BufferCommand(input[PassThruPrefix.Length..]);
                _context.Vm.Run();
                _context.Vm.PrintOutputBuffer();
                continue;
            }

            if (!ParseInput(input, out var cmd, out var arg))
            {
                continue;
            }
            
            if (_commandTable.TryGetValue(cmd, out var command))
            {
                command.Handler.Invoke(arg);
                continue;
            }

            Log(line: $"Command not recognized [{cmd}]");
        }
    }

    private void Help(string _)
    {
        Log(line: "Available commands:");
        foreach (var command in _commandTable.Values)
        {
            Log(line: $"-{command.Name}: {command.Desc}");
        }
    }

    private void New(string _)
    {
        _context.Vm = Vm.New();
    }
    
    private void Run(string _)
    {
        _context.Vm.Run();
        _context.Vm.PrintOutputBuffer();
    }

    private void Save(string path)
    {
        _context.Vm.SaveToFile(path);
        Log(line: $"VM state saved to {path}");
    }

    private void Load(string path)
    {
        _context.Vm = Vm.LoadFromFile(path);
        Log(line: $"VM state loaded from {path}");
    }
    
    private static bool ParseInput(string input, out string cmd, out string arg)
    {
        cmd = string.Empty;
        arg = string.Empty;

        if (string.IsNullOrEmpty(input))
        {
            return false;
        }
        
        var index = input.IndexOf(' ');
        if (index < 0 || index == input.Length - 1)
        {
            cmd = input;
            return true;
        }

        cmd = input[..index];
        arg = input[(index + 1)..];
        return true;
    }
    
    private Dictionary<string, Command> BuildCommandTable()
    {
        return EnumerateCommands()
            .ToDictionary(keySelector: command => command.Name);
    }

    private IEnumerable<Command> EnumerateCommands()
    {
        yield return new Command(
            name: HelpCmdId,
            desc: "print all available commands",
            handler: Help);
        yield return new Command(
            name: "clear",
            desc: "clear the console",
            handler: _ => Console.Clear());
        yield return new Command(
            name: "new",
            desc: "create a new VM instance, loaded with the challenge binary",
            handler: New);
        yield return new Command(
            name: "run", 
            desc: "run the current VM instance",
            handler: Run);
        yield return new Command(
            name: "save", 
            desc: "serialize the current VM instance state to the specified path",
            handler: Save);
        yield return new Command(
            name: "load",
            desc: "replace the current VM instance with one whose state is deserialized from the specified path",
            handler: Load);
        yield return new Command(
            name: "solve-coin",
            desc: "solve the coin puzzle and print the solution",
            handler: _ => Log(line: CoinPuzzle.Solve()));
    }

    private static void Log(string? line = null)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(line);
        Console.ResetColor();
    }
}