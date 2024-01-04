using System.Runtime.Versioning;
using Synacor.Cli;

namespace Synacor;

[SupportedOSPlatform("windows")]
internal static class Program
{
    public static void Main()
    {
        var shell = new Shell();
        shell.Start();
    }
}