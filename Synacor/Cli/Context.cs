using Synacor.VirtualMachine;

namespace Synacor.Cli;

public class Context
{
    public Vm Vm { get; set; }

    public Context(Vm vm)
    {
        Vm = vm;
    }
}