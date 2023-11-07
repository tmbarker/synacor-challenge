namespace Synacor.VirtualMachine;

public partial class Vm
{
    private const ushort Unset = 0;
    private const ushort MaxMem = 32767;
    private const ushort MinReg = 32768;
    private const ushort MaxReg = 32775;
    private const ushort Modulus = 32768;
    private const ushort BitMask15 = 32767;

    private ushort _ip = Unset;
    private readonly Dictionary<Opcode, Operation> _vectors = new();
    private readonly Dictionary<ushort, ushort> _memory = new();
    private readonly Dictionary<ushort, ushort> _registers = new();
    private readonly Stack<ushort> _stack = new();
    private readonly Queue<char> _inputBuffer = new();
    
    public Vm()
    {
        BindVectors();
    }

    public void Load(ushort[] program)
    {
        _ip = Unset;
        _memory.Clear();
        _registers.Clear();
        _stack.Clear();
        _inputBuffer.Clear();

        for (var i = 0; i < program.Length; i++)
        {
            _memory[(ushort)i] = program[i];
        }
    }
    
    public Result Run()
    {
        var ec = Result.ok;
        while (!ec.Halted())
        {
            ec = _vectors[ReadOpcode()].Invoke();
        }
        
        return ec;
    }

    private ushort ReadIpLiteral()
    {
        return ReadMem(adr: _ip++);
    }
    
    private ushort ReadIpInterpreted()
    {
        var literal = ReadIpLiteral();
        var isRegister = literal is >= MinReg and <= MaxReg;

        return isRegister
            ? ReadReg(adr: literal)
            : literal;
    }

    private Opcode ReadOpcode()
    {
        var literal = ReadIpLiteral();
        var opcode = (Opcode)literal;

        return Enum.IsDefined(typeof(Opcode), opcode)
            ? opcode
            : throw new InvalidOpcodeException(literal);
    }
    
    private void WriteVal(ushort adr, ushort val)
    {
        if (adr is >= MinReg and <= MaxReg)
        {
            WriteReg(adr, val);
        }
        else
        {
            WriteMem(adr, val);
        }
    }
    
    private ushort ReadReg(ushort adr)
    {
        return _registers.TryGetValue(CheckedReg(adr), out var value)
            ? value
            : Unset;
    }
    
    private ushort ReadMem(ushort adr)
    {
        return _memory.TryGetValue(CheckedAdr(adr), out var value)
            ? value
            : Unset;
    }
    
    private void WriteReg(ushort reg, ushort val)
    {
        _registers[CheckedReg(reg)] = val;
    }
    
    private void WriteMem(ushort adr, ushort val)
    {
        _memory[CheckedAdr(adr)] = val;
    }

    private static ushort CheckedReg(ushort reg)
    {
        return reg is >= MinReg and <= MaxReg
            ? reg
            : throw new InvalidRegisterException(reg);
    }
    
    private static ushort CheckedAdr(ushort adr)
    {
        return adr <= MaxMem
            ? adr
            : throw new InvalidMemoryException(adr);
    }
}