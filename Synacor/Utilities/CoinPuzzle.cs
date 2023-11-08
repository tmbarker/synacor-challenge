namespace Synacor.Utilities;

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
                return string.Join(", ", enumerated.Select(coin => coin.Name));
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

    private static IEnumerable<IEnumerable<T>> Permute<T>(IEnumerable<T> sequence)
    {
        var list = sequence.ToList();
        if (!list.Any())
        {
            yield return Enumerable.Empty<T>();
        }
        else
        {
            var startingElementIndex = 0;
            foreach (var startingElement in list)
            {
                var index = startingElementIndex;
                var remainingItems = list.Where((_, i) => i != index);

                foreach (var permutationOfRemainder in Permute(remainingItems))
                {
                    yield return permutationOfRemainder.Prepend(startingElement);
                }

                startingElementIndex++;
            }
        }
    }
}