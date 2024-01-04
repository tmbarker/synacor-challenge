namespace Synacor.VirtualMachine;

public partial class Vm
{
    private readonly HashSet<ushort> _breakpoints = [];
    
    public void SetIp(ushort ip)
    {
        _state.Ip = ip;
    }
    
    public void SetReg(ushort reg, ushort val)
    {
        switch (reg)
        {
            case >= MIN_REG and <= MAX_REG:
                WriteReg(adr: reg, val);
                break;
            case < NUM_REG:
                WriteReg(adr: (ushort)(MIN_REG + reg), val);
                break;
            default:
                throw new InvalidRegisterException(reg);
        }
    }

    public void AddBreakpoint(ushort adr)
    {
        _breakpoints.Add(adr);
    }

    public void ClearBreakpoint(ushort adr)
    {
        _breakpoints.Remove(adr);
    }
    
    public string GetState()
    {
        return _state.ToString();
    }
}