namespace Synacor.VirtualMachine;

public class InvalidMemoryException : Exception
{
    private const string MessageFormat = "Invalid address [{0}]";

    public InvalidMemoryException(ushort adr) : base(message: string.Format(MessageFormat, adr))
    {
    }
}

public class InvalidRegisterException : Exception
{
    private const string MessageFormat = "Invalid register [{0}]";

    public InvalidRegisterException(ushort reg) : base(message: string.Format(MessageFormat, reg))
    {
    }
}