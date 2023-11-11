namespace Synacor.VirtualMachine;

public partial class Vm
{
    public void SetIp(ushort ip)
    {
        _state.Ip = ip;
    }
    
    public void SetReg(ushort reg, ushort val)
    {
        switch (reg)
        {
            case >= MinReg and <= MaxReg:
                WriteReg(adr: reg, val);
                break;
            case < NumReg:
                WriteReg(adr: (ushort)(MinReg + reg), val);
                break;
            default:
                throw new InvalidRegisterException(reg);
        }
    }

    public string GetState()
    {
        return _state.ToString();
    }
}