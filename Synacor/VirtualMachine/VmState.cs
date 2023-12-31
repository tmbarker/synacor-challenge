using System.Text;

namespace Synacor.VirtualMachine;

public partial class Vm
{
    private class State
    {
        public ushort Ip { get; set; }
        public ushort[] Memory { get; } = new ushort[MEM_SIZE];
        public ushort[] Registers { get; } = new ushort[NUM_REG];
        public Stack<ushort> Stack { get; } = new();
        public Queue<char> InputBuffer { get; } = new();
        public Queue<char> OutputBuffer { get; } = new();

        private State()
        {
        }

        public State(IReadOnlyList<ushort> program)
        {
            for (var i = 0; i < program.Count; i++)
            {
                Memory[i] = program[i];
            }
        }

        public void Serialize(string path)
        {
            using var stream = File.Open(path, mode: FileMode.Create);
            using var writer = new BinaryWriter(stream);
            
            writer.Write(Ip);
            
            writer.Write(Memory.Length);
            foreach (var value in Memory)
            {
                writer.Write(value);
            }
            
            writer.Write(Registers.Length);
            foreach (var value in Registers)
            {
                writer.Write(value);
            }

            writer.Write(Stack.Count);
            foreach (var value in Stack.Reverse())
            {
                writer.Write(value);
            }

            writer.Write(InputBuffer.Count);
            foreach (var value in InputBuffer)
            {
                writer.Write(value);
            }
            
            writer.Write(OutputBuffer.Count);
            foreach (var value in OutputBuffer)
            {
                writer.Write(value);
            }
        }
        
        public static State Deserialize(string path)
        {
            var state = new State();
            
            using var stream = File.OpenRead(path);
            using var reader = new BinaryReader(stream);
            
            state.Ip = reader.ReadUInt16();

            var memSize = reader.ReadInt32();
            for (var i = 0; i < memSize; i++)
            {
                state.Memory[i] = reader.ReadUInt16();
            }

            var regSize = reader.ReadInt32();
            for (var i = 0; i < regSize; i++)
            {
                state.Registers[i] = reader.ReadUInt16();
            }

            var stackSize = reader.ReadInt32();
            for (var i = 0; i <stackSize; i++)
            {
                state.Stack.Push(item: reader.ReadUInt16());
            }

            var inBufferSize = reader.ReadInt32();
            for (var i = 0; i <inBufferSize; i++)
            {
                state.InputBuffer.Enqueue(item: reader.ReadChar());
            }
            
            var outBufferSize = reader.ReadInt32();
            for (var i = 0; i <outBufferSize; i++)
            {
                state.OutputBuffer.Enqueue(item: reader.ReadChar());
            }

            return state;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var op = Ip < Memory.Length
                ? $"{(Opcode)Memory[Ip]}"
                : "INVALID";

            sb.AppendLine($"{nameof(Ip)}: {Ip} [{op}]");
            sb.AppendLine($"{nameof(Registers)}: {Format(Registers.Select((val, i) => $"{i}: {val}"))}");
            sb.AppendLine($"{nameof(Stack)}: {Format(Stack)} <- Bottom");
            sb.AppendLine($"{nameof(InputBuffer)}: {Format(InputBuffer)}");
            sb.AppendLine($"{nameof(OutputBuffer)}: {Format(OutputBuffer)}");
            sb.Append($"{nameof(Memory)}: NA");
            
            return sb.ToString();
        }

        private static string Format<T>(IEnumerable<T> sequence) => string.Join(separator: ", ", sequence);
    }
}