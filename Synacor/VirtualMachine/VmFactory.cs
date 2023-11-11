using Synacor.Utilities;

namespace Synacor.VirtualMachine;

public partial class Vm
{
    public static Vm New()
    {
        var challengeBinary = BinaryUtil.ReadChallengeBinary();
        var state = new State(challengeBinary);
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