using Synacor.VirtualMachine;

namespace Synacor.Cli;

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
        PrintShellLog(StartMessage);
        PrintShellLog(HelpHint);
        PrintShellLog(QuitHint);
        PrintShellLog(PassThruHint);
        PrintShellLog();
        
        while (true)
        {
            var input = Console.ReadLine() ?? string.Empty;
            var passThru = input.StartsWith(PassThruPrefix);

            if (input == QuitCmdId)
            {
                PrintShellLog(EndedMessage);
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

            PrintShellLog($"Command not recognized [{cmd}]");
        }
    }

    private void Help(string _)
    {
        PrintShellLog("Available commands:");
        foreach (var command in _commandTable.Values)
        {
            PrintShellLog($"-{command.Name}: {command.Desc}");
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
    }

    private void Load(string path)
    {
        _context.Vm = Vm.LoadFromFile(path);
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
        if (index < 0)
        {
            cmd = input;
            return true;
        }

        cmd = input[..index];
        arg = input[index..];
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
    }

    private static void PrintShellLog(string? log = null)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(log);
        Console.ResetColor();
    }
}