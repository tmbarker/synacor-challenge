namespace Synacor.VirtualMachine;

public partial class Vm
{
    private const char NewLine = '\n';

    public void PrintOutputBuffer()
    {
        while (_state.OutputBuffer.Any())
        {
            Console.Write(_state.OutputBuffer.Dequeue());
        }
    }
    
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
}