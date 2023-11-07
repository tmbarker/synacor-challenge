namespace Synacor.VirtualMachine;

public partial class Vm
{
    private delegate Result Operation();
    
    private void BindVectors()
    {
        _vectors[Opcode.halt] = Halt;
        _vectors[Opcode.set]  = Set;
        _vectors[Opcode.push] = Push;
        _vectors[Opcode.pop]  = Pop;
        _vectors[Opcode.eq]   = Eq;
        _vectors[Opcode.gt]   = Gt;
        _vectors[Opcode.jmp]  = Jmp;
        _vectors[Opcode.jt]   = Jt;
        _vectors[Opcode.jf]   = Jf;
        _vectors[Opcode.add]  = Add;
        _vectors[Opcode.mult] = Mult;
        _vectors[Opcode.mod]  = Mod;
        _vectors[Opcode.and]  = And;
        _vectors[Opcode.or]   = Or;
        _vectors[Opcode.not]  = Not;
        _vectors[Opcode.rmem] = Rmem;
        _vectors[Opcode.wmem] = Wmem;
        _vectors[Opcode.call] = Call;
        _vectors[Opcode.ret]  = Ret;
        _vectors[Opcode.@out] = Out;
        _vectors[Opcode.@in]  = In;
        _vectors[Opcode.noop] = Noop;
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
        _stack.Push(item: ReadIpInterpreted());
        return Result.ok;
    }
    
    private Result Pop()
    {
        if (_stack.Count == 0)
        {
            throw new InvalidOperationException(message: $"Cannot execute {nameof(Opcode.pop)}: Stack is empty");
        }
        
        var a = ReadIpLiteral();
        var b = _stack.Pop();

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
        _ip = ReadIpInterpreted();
        return Result.ok;
    }
    
    private Result Jt()
    {
        var a = ReadIpInterpreted();
        var b = ReadIpInterpreted();

        if (a != 0)
        {
            _ip = b;
        }
        
        return Result.ok;
    }
    
    private Result Jf()
    {
        var a = ReadIpInterpreted();
        var b = ReadIpInterpreted();

        if (a == 0)
        {
            _ip = b;
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
        _stack.Push(_ip);
        _ip = a;
        return Result.ok;
    }
    
    private Result Ret()
    {
        if (_stack.Count == 0)
        {
            return Halt();
        }

        _ip = _stack.Pop();
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
        var c = _inputBuffer.Dequeue();

        WriteVal(adr: a, val: c);
        return Result.ok;
    }

    private static Result Noop()
    {
        return Result.ok;
    }

    private void EnsureInput()
    {
        if (!_inputBuffer.Any())
        {
            ReadInput();
        }
    }

    private void ReadInput()
    {
        foreach (var c in $"{Console.ReadLine()}\n")
        {
            _inputBuffer.Enqueue(c);
        }
    }
}