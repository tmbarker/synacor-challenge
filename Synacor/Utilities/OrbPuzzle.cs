namespace Synacor.Utilities;

/// <summary>
/// A utility class for solving the "Orb Puzzle", which entails pathfinding through a grid such that the
/// cumulative "cost" of the path matches the desired cost at the goal. This utility constructs the
/// commands needed to traverse such a path, they can be either printed or directly injected into a VM
/// instance.
/// </summary>
public static class OrbPuzzle
{
    private const int TargetWeight = 30;
    private static readonly string[,] Grid = {
        {  "*", "8",  "-",  "1" },
        {  "4", "*", "11",  "*" },
        {  "+", "4",  "-", "18" },
        { "22", "-",  "9",  "*" }
    };
    private static readonly Dictionary<Vector2D, string> Commands = new()
    {
        { Vector2D.Up,    "north" },
        { Vector2D.Down,  "south" },
        { Vector2D.Left,   "west" },
        { Vector2D.Right,  "east" }
    };
    
    public static IEnumerable<string> Solve()
    {
        var map = BuildMap(grid: Grid);
        var startPos = new Vector2D(X: 0, Y: 0);
        var endPos   = new Vector2D(X: 3, Y: 3);
        var startState = new State(Pos: startPos, Weight: map.Literals[startPos]);
        var endState   = new State(Pos: endPos,   Weight: TargetWeight);
        
        var queue = new Queue<State>(collection: [startState]);
        var visited = new HashSet<State>(collection: [startState]);
        var paths = new Dictionary<State, List<string>> { { startState, [] } };

        while (queue.Count > 0)
        {
            var state = queue.Dequeue();
            if (state == endState)
            {
                return paths[state];
            }

            if (state.Pos == endPos)
            {
                continue;
            }
            
            foreach (var step1 in Vector2D.Dirs)
            foreach (var step2 in Vector2D.Dirs)
            {
                var operatorPos = state.Pos + step1;
                var literalPos = operatorPos + step2;

                if (operatorPos == startPos ||
                    literalPos == startPos ||
                    !map.Operators.ContainsKey(operatorPos) ||
                    !map.Literals.ContainsKey(literalPos))
                {
                    continue;
                }

                var adjacent = new State(Pos: literalPos, Weight: map.Operators[operatorPos] switch
                {
                    "+" => state.Weight + map.Literals[literalPos],
                    "-" => state.Weight - map.Literals[literalPos],
                    "*" => state.Weight * map.Literals[literalPos],
                    _ => throw new ArgumentOutOfRangeException()
                });
                
                if (visited.Add(adjacent))
                {
                    queue.Enqueue(adjacent);
                    paths[adjacent] = BuildPath(paths[state], step1, step2);
                }
            }
        }

        throw new Exception(message: "No solution found, validate the grid");
    }

    private static List<string> BuildPath(List<string> previous, Vector2D step1, Vector2D step2)
    {
        return
        [
            ..previous, 
            Commands[step1],
            Commands[step2]
        ];
    }
    
    private static Map BuildMap(string[,] grid)
    {
        var literals  = new Dictionary<Vector2D, int>();
        var operators = new Dictionary<Vector2D, string>();
        var rows = grid.GetLength(dimension: 0);
        var cols = grid.GetLength(dimension: 1);
        
        for (var y = 0; y < rows; y++)
        for (var x = 0; x < cols; x++)
        {
            if (int.TryParse(grid[rows - y - 1, x], out var literal))
            {
                literals[new Vector2D(x, y)] = literal;
            }
            else
            {
                operators[new Vector2D(x, y)] = grid[rows - y - 1, x];
            }
        }

        return new Map(literals, operators);
    }
}

public readonly record struct State(Vector2D Pos, int Weight);
public readonly record struct Map(Dictionary<Vector2D, int> Literals, Dictionary<Vector2D, string> Operators);

public readonly record struct Vector2D(int X, int Y)
{
    public static readonly Vector2D Zero  = new(X:  0, Y:  0);
    public static readonly Vector2D Up    = new(X:  0, Y:  1);
    public static readonly Vector2D Down  = new(X:  0, Y: -1);
    public static readonly Vector2D Left  = new(X: -1, Y:  0);
    public static readonly Vector2D Right = new(X:  1, Y:  0);
    
    public static IEnumerable<Vector2D> Dirs { get; } = Zero.GetAdjacentSet();
    
    public static Vector2D operator +(Vector2D a, Vector2D b)
    {
        return new Vector2D(X: a.X + b.X, Y: a.Y + b.Y);
    }

    private IEnumerable<Vector2D> GetAdjacentSet()
    {
        return [this + Up, this + Down, this + Left, this + Right];
    }
}