namespace Synacor.VirtualMachine;

public partial class Vm
{
    private delegate Result Operation();
    
    private Dictionary<Opcode, Operation> BuildVectorTable()
    {
        return new Dictionary<Opcode, Operation>
        {
            [Opcode.halt] = Halt,
            [Opcode.set]  = Set,
            [Opcode.push] = Push,
            [Opcode.pop]  = Pop,
            [Opcode.eq]   = Eq,
            [Opcode.gt]   = Gt,
            [Opcode.jmp]  = Jmp,
            [Opcode.jt]   = Jt,
            [Opcode.jf]   = Jf,
            [Opcode.add]  = Add,
            [Opcode.mult] = Mult,
            [Opcode.mod]  = Mod,
            [Opcode.and]  = And,
            [Opcode.or]   = Or,
            [Opcode.not]  = Not,
            [Opcode.rmem] = Rmem,
            [Opcode.wmem] = Wmem,
            [Opcode.call] = Call,
            [Opcode.ret]  = Ret,
            [Opcode.@out] = Out,
            [Opcode.@in]  = In,
            [Opcode.noop] = Noop
        };
    }
    
    private static Result Halt()
    {
        return Result.halted;
    }

    private Result Set()
    {
        var a = ReadIpLiteral();
        var b = ReadIpInterpreted();

        WriteVal(adr: a, val: b);
        return Result.ok;
    }
    
    private Result Push()
    {
        _state.Stack.Push(item: ReadIpInterpreted());
        return Result.ok;
    }
    
    private Result Pop()
    {
        if (_state.Stack.Count == 0)
        {
            throw new InvalidOperationException(message: $"Cannot execute {nameof(Opcode.pop)}: Stack is empty");
        }
        
        var a = ReadIpLiteral();
        var b = _state.Stack.Pop();

        WriteVal(adr: a, val: b);
        return Result.ok;
    }
    
    private Result Eq()
    {
        var a = ReadIpLiteral();
        var b = ReadIpInterpreted();
        var c = ReadIpInterpreted();

        WriteVal(adr: a, val: (ushort)(b == c ? 1 : 0));
        return Result.ok;
    }
    
    private Result Gt()
    {
        var a = ReadIpLiteral();
        var b = ReadIpInterpreted();
        var c = ReadIpInterpreted();

        WriteVal(adr: a, val: (ushort)(b > c ? 1 : 0));
        return Result.ok;
    }
    
    private Result Jmp()
    {
        _state.Ip = ReadIpInterpreted();
        return Result.ok;
    }
    
    private Result Jt()
    {
        var a = ReadIpInterpreted();
        var b = ReadIpInterpreted();

        if (a != 0)
        {
            _state.Ip = b;
        }
        
        return Result.ok;
    }
    
    private Result Jf()
    {
        var a = ReadIpInterpreted();
        var b = ReadIpInterpreted();

        if (a == 0)
        {
            _state.Ip = b;
        }
        
        return Result.ok;
    }
    
    private Result Add()
    {
        var a = ReadIpLiteral();
        var b = ReadIpInterpreted();
        var c = ReadIpInterpreted();

        WriteVal(adr: a, val: (ushort)((b + c) % Modulus));
        return Result.ok;
    }
    
    private Result Mult()
    {
        var a = ReadIpLiteral();
        var b = ReadIpInterpreted();
        var c = ReadIpInterpreted();

        WriteVal(adr: a, val: (ushort)((b * c) % Modulus));
        return Result.ok;
    }
    
    private Result Mod()
    {
        var a = ReadIpLiteral();
        var b = ReadIpInterpreted();
        var c = ReadIpInterpreted();

        WriteVal(adr: a, val: (ushort)((b % c) % Modulus));
        return Result.ok;
    }
    
    private Result And()
    {
        var a = ReadIpLiteral();
        var b = ReadIpInterpreted();
        var c = ReadIpInterpreted();

        WriteVal(adr: a, val: (ushort)((b & c) % Modulus));
        return Result.ok;
    }
    
    private Result Or()
    {
        var a = ReadIpLiteral();
        var b = ReadIpInterpreted();
        var c = ReadIpInterpreted();

        WriteVal(adr: a, val: (ushort)((b | c) % Modulus));
        return Result.ok;
    }
    
    private Result Not()
    {
        var a = ReadIpLiteral();
        var b = ReadIpInterpreted();

        WriteVal(adr: a, val: (ushort)((~b & BitMask15) % Modulus));
        return Result.ok;
    }
    
    private Result Rmem()
    {
        var a = ReadIpLiteral();
        var b = ReadIpInterpreted();

        WriteVal(adr: a, val: ReadMem(b));
        return Result.ok;
    }
    
    private Result Wmem()
    {
        var a = ReadIpInterpreted();
        var b = ReadIpInterpreted();

        WriteMem(adr: a, val: b);
        return Result.ok;
    }
    
    private Result Call()
    {
        var a = ReadIpInterpreted();
        _state.Stack.Push(_state.Ip);
        _state.Ip = a;
        return Result.ok;
    }
    
    private Result Ret()
    {
        if (_state.Stack.Count == 0)
        {
            return Halt();
        }

        _state.Ip = _state.Stack.Pop();
        return Result.ok;
    }
    
    private Result Out()
    {
        var instr = ReadIpInterpreted();
        var @char = Convert.ToChar(instr);

        Console.Write(@char);
        return Result.ok;
    }
    
    private Result In()
    {
        EnsureInput();

        var a = ReadIpLiteral();
        var c = _state.InputBuffer.Dequeue();

        WriteVal(adr: a, val: c);
        return Result.ok;
    }

    private static Result Noop()
    {
        return Result.ok;
    }
}