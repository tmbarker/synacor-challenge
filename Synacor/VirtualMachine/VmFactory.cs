namespace Synacor.VirtualMachine;

public partial class Vm
{
    public static Vm Create(ushort[] program)
    {
        var state = new State(program);
        var vm = new Vm(state);
        
        return vm;
    }

    public static Vm LoadFromFile(string path)
    {
        var state = State.Deserialize(path);
        var vm = new Vm(state);

        return vm;
    }

    public void SaveToFile(string path)
    {
        _state.Serialize(path);
    }
}