using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using Synacor.Utilities;
using Synacor.VirtualMachine;

namespace Synacor.Cli;

/// <summary>
/// A shell for working with Synacor <see cref="Vm"/> instances
/// </summary>
[SupportedOSPlatform("windows")]
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
        LogInfo(StartMessage);
        LogInfo(HelpHint);
        LogInfo(QuitHint);
        LogInfo(PassThruHint);
        LogInfo();
        
        while (true)
        {
            var input = Console.ReadLine() ?? string.Empty;
            var passThru = input.StartsWith(PassThruPrefix);

            if (input == QuitCmdId)
            {
                LogInfo(EndedMessage);
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

            LogError(error: $"Command not recognized [{cmd}]");
        }
    }

    private void Help(string _)
    {
        LogInfo(info: "Available commands:");
        foreach (var command in _commandTable.Values)
        {
            LogInfo(info: $"- {command.Name}");
            LogInfo(info: $"  syntax: {command.Syntax}");
            LogInfo(info: $"  desc:   {command.Desc}");
            LogInfo();
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
        LogInfo(info: $"VM state saved to {path}");
    }

    private void Load(string path)
    {
        _context.Vm = Vm.LoadFromFile(path);
        LogInfo(info: $"VM state loaded from {path}");
    }

    private void SetIp(string arg)
    {
        try
        {
            var matches = Regex.Matches(arg, pattern: @"\d+");
            var number = matches
                .Select(m => ushort.Parse(m.Value))
                .First();
        
            _context.Vm.SetIp(ip: number);
        }
        catch
        {
            LogError(error: $"Unable to parse IP[{arg}]");
        }
    }
    
    private void SetReg(string arg)
    {
        try
        {
            var matches = Regex.Matches(arg, pattern: @"\d+");
            var numbers = matches
                .Select(m => ushort.Parse(m.Value))
                .ToArray();

            _context.Vm.SetReg(reg: numbers[0], val: numbers[1]);
        }
        catch
        {
            LogError(error: $"Unable to parse register and value [{arg}]");
        }
    }

    private void SetBreakpoint(string arg)
    {
        try
        {
            _context.Vm.AddBreakpoint(adr: ushort.Parse(arg));
        }
        catch
        {
            LogError(error: $"Unable to parse IP [{arg}]");
        }
    }

    private void ClearBreakpoint(string arg)
    {
        try
        {
            _context.Vm.ClearBreakpoint(adr: ushort.Parse(arg));
        }
        catch
        {
            LogError(error: $"Unable to parse IP [{arg}]");
        }
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
            syntax: HelpCmdId,
            desc: "print all available commands",
            handler: Help);
        yield return new Command(
            name: "clear",
            syntax: "clear",
            desc: "clear the console",
            handler: _ => Console.Clear());
        yield return new Command(
            name: "new",
            syntax: "new",
            desc: "create a new VM instance, loaded with the challenge binary",
            handler: New);
        yield return new Command(
            name: "run", 
            syntax: "run",
            desc: "run the current VM instance",
            handler: Run);
        yield return new Command(
            name: "step",
            syntax: "step",
            desc: "step the current VM instance over a single instruction",
            handler: _ => _context.Vm.Step());
        yield return new Command(
            name: "save",
            syntax: "save [path]",
            desc: "serialize the current VM instance state to the specified path",
            handler: Save);
        yield return new Command(
            name: "load",
            syntax: "load [path]",
            desc: "replace the current VM instance with one whose state is deserialized from the specified path",
            handler: Load);
        yield return new Command(
            name: "set-reg",
            syntax: "set-reg [reg] [value]",
            desc: "set the value stored in the specified register",
            handler: SetReg);
        yield return new Command(
            name: "set-ip",
            syntax: "set-ip [value]",
            desc: "set the instruction pointer to the specified value",
            handler: SetIp);
        yield return new Command(
            name: "set-breakpoint",
            syntax: "set-breakpoint [address]",
            desc: "Set a breakpoint at the specified IP address",
            handler: SetBreakpoint);
        yield return new Command(
            name: "clear-breakpoint",
            syntax: "clear-breakpoint [address]",
            desc: "Clear any breakpoint from the specified IP address",
            handler: ClearBreakpoint);
        yield return new Command(
            name: "get-state",
            syntax: "get-state",
            desc: "print the state of the VM registers, IP, stack, and buffers (but not the main memory)",
            handler: _ => LogData(data: _context.Vm.GetState()));
        yield return new Command(
            name: "get-output",
            syntax: "get-output",
            desc: "print (and purge) the current VM instance output buffer",
            handler: _ => _context.Vm.PrintOutputBuffer());
        yield return new Command(
            name: "solve-coin",
            syntax: "solve-coin",
            desc: "solve the coin puzzle and print the solution",
            handler: _ => LogInfo(info: CoinPuzzle.Solve()));
        yield return new Command(
            name: "solve-teleporter",
            syntax: "solve-teleporter",
            desc: "solve the teleporter puzzle and print the solution",
            handler: _ => LogInfo(info: TeleporterPuzzle.Solve()));
        yield return new Command(
            name: "dsm",
            syntax: "dsm [path]",
            desc: "disassemble the challenge binary and save it to the specified path",
            handler: BinaryUtil.DisassembleChallengeBinary);
    }

    private static void LogInfo(string? info = null) => Log(log: info, color: ConsoleColor.Green);
    private static void LogError(string? error = null) => Log(log: error, color: ConsoleColor.Red);
    private static void LogData(string? data = null) => Log(log: data, color: ConsoleColor.Yellow);

    private static void Log(string? log, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(log);
        Console.ResetColor();
    }
}