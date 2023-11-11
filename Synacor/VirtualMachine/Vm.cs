namespace Synacor.VirtualMachine;

/// <summary>
/// A Virtual Machine adhering to the challenge architecture specification
/// </summary>
public partial class Vm
{
    private const ushort MemSize = 32768;
    private const ushort NumReg = 8;
    private const ushort Modulus = 32768;
    private const ushort BitMask15 = 32767;
    
    public const ushort MinReg = MemSize;
    public const ushort MaxReg = MinReg + NumReg - 1;
    
    private readonly State _state;
    private readonly Dictionary<Opcode, Operation> _vectors;
    
    public Result Run()
    {
        var status = Result.ok;
        while (status.Ok())
        {
            status = _vectors[ReadOpcode()].Invoke();
        }
        
        return status;
    }
    
    private Vm(State state)
    {
        _state = state;
        _vectors = BuildVectorTable();
    }

    private ushort ReadIpLiteral()
    {
        return ReadMem(adr: _state.Ip++);
    }
    
    private ushort ReadIpInterpreted()
    {
        var literal = ReadIpLiteral();
        var isRegister = IsRegister(adr: literal);

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
        if (IsRegister(adr))
        {
            WriteReg(adr, val);
        }
        else
        {
            WriteMem(adr, val);
        }
    }

    private static bool IsRegister(ushort adr)
    {
        return adr is >= MinReg and <= MaxReg;
    }
    
    private ushort ReadReg(ushort adr)
    {
        return _state.Registers[CheckedReg(adr)];
    }
    
    private ushort ReadMem(ushort adr)
    {
        return _state.Memory[CheckedAdr(adr)];
    }
    
    private void WriteReg(ushort adr, ushort val)
    {
        _state.Registers[CheckedReg(adr)] = val;
    }
    
    private void WriteMem(ushort adr, ushort val)
    {
        _state.Memory[CheckedAdr(adr)] = val;
    }

    private static int CheckedReg(ushort adr)
    {
        return IsRegister(adr)
            ? adr - MinReg
            : throw new InvalidRegisterException(adr);
    }
    
    private static int CheckedAdr(ushort adr)
    {
        return adr < MemSize
            ? adr
            : throw new InvalidMemoryException(adr);
    }
}