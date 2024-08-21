using System.Runtime.CompilerServices;

namespace Synacor.Utilities;

/// <summary>
/// A utility class for solving the "Teleporter Puzzle", which entails determining the value which should
/// be written to the eighth register before disabling the confirmation mechanism and using the teleporter
/// again
/// </summary>
public static class TeleporterPuzzle
{
    // ReSharper disable InconsistentNaming
    private const int  RECURSIVE_STACK_REQ = 20 * 1024 * 1024;
    private const uint UINT_1 = 1u;
    private const uint UINT_32767 = 32767u;
    private const uint UINT_MODULUS = 32768u;
    private const uint UINT_TARGET = 6u;
    // ReSharper restore InconsistentNaming
    
    public static string Solve()
    {
        var result = 0u;
        var thread = new Thread(Search, maxStackSize: RECURSIVE_STACK_REQ);

        Console.WriteLine("Solving teleporter puzzle, this will take a while...");
        thread.Start();
        thread.Join();
        
        return result.ToString();

        void Search()
        {
            for (var r8 = 0u; r8 < UINT_MODULUS; r8++)
            {
                if (Fn6049(x: r8, m: 4, n: 1, memo: []) == UINT_TARGET)
                {
                    result = r8;
                    break;
                }
            }
        }
    }
    
    /// <summary>
    /// This method was written by analyzing the challenge binary disassembly. The
    /// "Confirmation mechanism" for my binary began at IP 6049, and is a slightly
    /// modified version of the 2-ary Ackermann function.
    /// </summary>
    private static uint Fn6049(uint x, uint m, uint n, Dictionary<(uint, uint), uint> memo)
    {
        if (!memo.ContainsKey((m, n)))
        {
            memo[(m, n)] = (m, n) switch
            {
                (m: 0, n: _) => Inc(n),
                (m: _, n: 0) => Fn6049(x, m: Dec(m), n: x, memo),
                (m: _, n: _) => Fn6049(x, m: Dec(m), n: Fn6049(x, m, n: Dec(n), memo), memo)
            };
        }

        return memo[(m, n)];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint Inc(uint a) => (a + UINT_1) % UINT_MODULUS;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint Dec(uint a) => (a + UINT_32767) % UINT_MODULUS;
}