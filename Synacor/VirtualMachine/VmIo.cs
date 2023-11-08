namespace Synacor.VirtualMachine;

public partial class Vm
{
    private const char NewLine = '\n';
    
    public void BufferCommand(string command)
    {
        foreach (var c in command)
        {
            _state.InputBuffer.Enqueue(c);
        }

        if (!command.EndsWith(NewLine))
        {
            _state.InputBuffer.Enqueue(item: NewLine);
        }
    }

    public void BufferCommands(IEnumerable<string> commands)
    {
        foreach (var command in commands)
        {
            BufferCommand(command);
        }
    }
    
    private void EnsureInput()
    {
        if (!_state.InputBuffer.Any())
        {
            ReadCommand();
        }
    }

    private void ReadCommand()
    {
        BufferCommand(command: $"{Console.ReadLine()}{NewLine}");
    }
}