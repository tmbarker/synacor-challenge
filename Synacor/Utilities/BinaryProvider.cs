namespace Synacor.Utilities;

public static class BinaryProvider
{
    private const string RelativeBinaryPath = @"Resources\challenge.bin";
    
    public static ushort[] Read()
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, RelativeBinaryPath);
        var stream = File.OpenRead(path);
        var reader = new BinaryReader(stream);
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
}