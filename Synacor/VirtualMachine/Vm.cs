namespace Synacor.VirtualMachine;

public partial class Vm
{
    private const ushort Unset = 0;
    private const ushort MaxAdr = 32775;
    private const ushort NumReg = 8;
    private const ushort MinReg = MaxAdr - NumReg;
    private const ushort MaxReg = MinReg + NumReg;
    private const ushort Modulus = 32768;

    private ushort _ip = Unset;
    private ushort[] _program = Array.Empty<ushort>();
    
    private readonly Dictionary<Opcode, Action> _vectors = new();
    private readonly Dictionary<ushort, ushort> _memory = new();
    private readonly Stack<ushort> _stack = new();
    
    public Vm()
    {
        BindVectors();
    }

    public void Execute(ushort[] program)
    {
        _ip = 0;
        _program = program;
        _memory.Clear();
        _stack.Clear();

        while (_ip < program.Length)
        {
            _vectors[ReadOp()].Invoke();
        }
    }
    
    private ushort ReadInstr()
    {
        var literal = _program[_ip++];
        var register = literal is >= MinReg and <= MaxReg;

        return register
            ? ReadMem(literal)
            : literal;
    }

    private Opcode ReadOp()
    {
        return (Opcode)ReadInstr();
    }
    
    private void WriteMem(ushort adr, ushort value)
    {
        _memory[CheckedAdr(adr)] = value;
    }

    private ushort ReadMem(ushort adr)
    {
        return _memory.TryGetValue(CheckedAdr(adr), out var value)
            ? value
            : Unset;
    }
    
    private static ushort CheckedAdr(ushort adr)
    {
        return adr <= MaxAdr
            ? adr
            : throw new InvalidAddressException(adr);
    }
}