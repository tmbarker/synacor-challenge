namespace Synacor.VirtualMachine;

public class InvalidMemoryException : Exception
{
    public InvalidMemoryException(ushort value) : base(message: $"Invalid address [{value}]")
    {
    }
}

public class InvalidRegisterException : Exception
{
    public InvalidRegisterException(ushort value) : base(message: $"Invalid register [{value}]")
    {
    }
}

public class InvalidOpcodeException : Exception
{
    public InvalidOpcodeException(ushort value) : base(message: $"Invalid opcode [{value}]")
    {
    }
}