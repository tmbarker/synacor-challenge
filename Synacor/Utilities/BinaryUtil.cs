using Synacor.VirtualMachine;

namespace Synacor.Utilities;

public static class BinaryUtil
{
    private const string ExecutableRelativeResourcesPath = "Resources";
    private const string ChallengeBinaryFilename = "challenge.bin";

    private static readonly Dictionary<Vm.Opcode, int> OpcodeArgsCountMap = new()
    {
        { Vm.Opcode.halt, 0 },
        { Vm.Opcode.set,  2 },
        { Vm.Opcode.push, 1 },
        { Vm.Opcode.pop,  1 },
        { Vm.Opcode.eq,   3 },
        { Vm.Opcode.gt,   3 },
        { Vm.Opcode.jmp,  1 },
        { Vm.Opcode.jt,   2 },
        { Vm.Opcode.jf,   2 },
        { Vm.Opcode.add,  3 },
        { Vm.Opcode.mult, 3 },
        { Vm.Opcode.mod,  3 },
        { Vm.Opcode.and,  3 },
        { Vm.Opcode.or,   3 },
        { Vm.Opcode.not,  2 },
        { Vm.Opcode.rmem, 2 },
        { Vm.Opcode.wmem, 2 },
        { Vm.Opcode.call, 1 },
        { Vm.Opcode.ret,  0 },
        { Vm.Opcode.@out, 1 },
        { Vm.Opcode.@in,  1 },
        { Vm.Opcode.noop, 0 }
    };
    
    public static ushort[] ReadChallengeBinary()
    {
        var path = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            ExecutableRelativeResourcesPath,
            ChallengeBinaryFilename);
        
        using var stream = File.OpenRead(path);
        using var reader = new BinaryReader(stream);
        var length = stream.Length / 2;
        var program = new ushort[length];

        for (var i = 0; i < length; i++)
        {
            program[i] = reader.ReadUInt16();
        }

        stream.Dispose();
        reader.Dispose();
        
        return program;
    }

    public static void DisassembleChallengeBinary(string path)
    {
        using var stream = File.Open(path, mode: FileMode.Create);
        using var writer = new StreamWriter(stream);
        var instructions = ReadChallengeBinary();

        for (var adr = 0; adr < instructions.Length;)
        {
            var op = (Vm.Opcode)instructions[adr];
            if (!OpcodeArgsCountMap.TryGetValue(op, out var n))
            {
                adr++;
                continue;
            }

            var adrChunk = PadToLength(str: $"{adr}:", len: 7, front: false);
            var opChunk = PadToLength(str: $"{op}", len: 4, front: true);
            writer.Write(adrChunk);
            writer.Write(opChunk);
            
            for (var offset = 1; offset <= n; offset++)
            {
                var argAdr = adr + offset;
                var argToken = FormatArg(val: instructions[argAdr], op);
                var argChunk = PadToLength(str: argToken, len: 8, front: true); 
                writer.Write(argChunk);
            }

            writer.Write('\n');
            adr += n + 1;
        }
        
        writer.Dispose();
        stream.Dispose();
    }

    private static string PadToLength(string str, int len, bool front)
    {
        if (str.Length > len)
        {
            throw new Exception();
        }
        
        var delta = len - str.Length;
        var padding = delta > 0
            ? new string(c: ' ', count: delta)
            : string.Empty;

        return front
            ? $"{padding}{str}"
            : $"{str}{padding}";
    }

    private static string FormatArg(ushort val, Vm.Opcode opcode)
    {
        //  If the arg corresponds to a register show so explicitly
        //
        if (val is >= Vm.MinReg and <= Vm.MaxReg)
        {
            return $"reg[{val - Vm.MinReg}]";
        }
        
        //  If the opcode is @out, simply show the ascii char
        //
        if (opcode == Vm.Opcode.@out)
        {
            var @char = Convert.ToChar(val);
            return @char != '\n'
                ? new string(c: @char, count: 1)
                : @"\n";
        }
        
        return $"{val}";
    }
}