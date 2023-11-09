namespace Synacor.Utilities;

/// <summary>
/// A utility class for solving the "Coin Puzzle", which entails determining the required permutation of
/// coins to satisfy an equation found in the challenge binary
/// </summary>
public static class CoinPuzzle
{
    private readonly record struct Coin(string Name, int Value);
    private static readonly List<Coin> Coins = new()
    {
        new Coin(Name: "red", Value: 2),
        new Coin(Name: "corroded", Value: 3),
        new Coin(Name: "shiny", Value: 5),
        new Coin(Name: "concave", Value: 7),
        new Coin(Name: "blue", Value: 9)
    };
    
    public static string Solve()
    {
        foreach (var permutation in Permute(Coins))
        {
            var enumerated = permutation.ToList();
            if (Check(enumerated))
            {
                return string.Join(separator: ", ", enumerated.Select(coin => coin.Name));
            }
        }

        return "No solution exists";
    } 
    
    private static bool Check(IReadOnlyList<Coin> coins)
    {
        if (coins.Count != 5)
        {
            throw new ArgumentException(message: "This puzzle requires 5 coins");
        }
        
        var a = coins[0].Value;
        var b = coins[1].Value;
        var c = coins[2].Value;
        var d = coins[3].Value;
        var e = coins[4].Value;

        return a + b * c * c + d * d * d - e == 399;
    }

    private static IEnumerable<IEnumerable<Coin>> Permute(IEnumerable<Coin> sequence)
    {
        var enumerated = sequence.ToList();
        if (!enumerated.Any())
        {
            yield return Enumerable.Empty<Coin>();
        }
        else
        {
            var startingCoinIndex = 0;
            foreach (var startingCoin in enumerated)
            {
                var index = startingCoinIndex;
                var remaining = enumerated.Where((_, i) => i != index);

                foreach (var permutationOfRemainder in Permute(remaining))
                {
                    yield return permutationOfRemainder.Prepend(startingCoin);
                }

                startingCoinIndex++;
            }
        }
    }
}