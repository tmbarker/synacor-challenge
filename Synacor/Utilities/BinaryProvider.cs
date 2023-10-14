namespace Synacor.Utilities;

public static class BinaryProvider
{
    private const string RelativeBinaryPath = @"Resources\challenge.bin";
    
    public static ushort[] Read()
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, RelativeBinaryPath);
        var fs = new FileStream(path, mode: FileMode.Open, access: FileAccess.Read);
        var br = new BinaryReader(fs);
        var length = fs.Length / 2;
        var program = new ushort[length];

        for (var i = 0; i < length; i++)
        {
            program[i] = br.ReadUInt16();
        }

        fs.Dispose();
        br.Dispose();
        
        return program;
    }
}