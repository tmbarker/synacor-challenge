namespace Synacor.VirtualMachine;

public class InvalidAddressException : Exception
{
    private const string MessageFormat = "Invalid address [{0}]";

    public InvalidAddressException(ushort adr) : base(message: string.Format(MessageFormat, adr))
    {
    }
}