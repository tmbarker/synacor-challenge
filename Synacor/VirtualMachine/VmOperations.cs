namespace Synacor.VirtualMachine;

public partial class Vm
{
    private enum Opcode
    {
        halt =  0,
        set  =  1,
        push =  2,
        pop  =  3,
        eq   =  4,
        gt   =  5,
        jmp  =  6,
        jt   =  7,
        jf   =  8,
        add  =  9,
        mult = 10,
        mod  = 11,
        and  = 12,
        or   = 13,
        not  = 14,
        rmem = 15,
        wmem = 16,
        call = 17,
        ret  = 18,
        @out = 19,
        @in  = 20,
        noop = 21
    }

    private void BindVectors()
    {
        _vectors[Opcode.halt] = Halt;
        _vectors[Opcode.@out] = Out;
        _vectors[Opcode.noop] = Noop;
    }
    
    private void Halt()
    {
        _ip = (ushort)_program.Length;
    }

    private void Out()
    {
        var instr = ReadInstr();
        var @char = Convert.ToChar(instr);

        Console.Write(@char);
    }

    private void Noop()
    {
        _ip++;
    }
}