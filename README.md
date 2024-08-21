# synacor-challenge
This repo contains my C# .NET solution to the Synacor OSCON 2012 Challenge. The Synacor Challenge was issued by Eric Wastl (of [Advent of Code](https://adventofcode.com/) fame) in 2012 and entails creating a virtual machine that adheres to a provided architecture specification. As you implement your virtual machine, and solve puzzles with it, you will discover __*codes*__. 

In the past these __*codes*__ were to be entered and verified on the challenge website, however, that is no longer available.
* A copy of the challenge architecture specification can be found [here](https://github.com/tmbarker/synacor-challenge/blob/main/Synacor/Resources/arch-spec.txt).
* A copy of a challenge binary can be found [here](https://github.com/tmbarker/synacor-challenge/blob/main/Synacor/Resources/challenge.bin).
* My __*codes*__, associated with the above binary, can be found [here](https://github.com/tmbarker/synacor-challenge/blob/main/Synacor/codes.txt). An explanation of how I acquired each code follows below.

## Code #1: That Was Easy
The first thing I did was open the provided architecture specification, `arch-spec.txt`, in the hints section was the following text:
```
Here's a code for the challenge website: LDOb7UGhTi
```

## Code #2: A Fledgling VM
Another hint in the architecture specification, `arch-spec.txt`, suggested the following:
```
Start with operations 0, 19, and 21.
```
These opcodes are described further down as follows:
```
halt: 0 
  stop execution and terminate the program
out: 19 a
  write the character represented by ascii code <a> to the terminal
noop: 21
  no operation
```
Implementing these three opcodes, and running the provided binary with my fledgling VM, was enough to see the following text printed to the console:
```
Welcome to the Synacor OSCON 2012 Challenge!
Please record your progress by putting codes like
this one into the challenge website: ImoFztWQCvxj
```

## Code #3: A Maturing VM
The next thing I did was implement the remaining opcodes, of which there are a total of 22. The provided binary is ingeniously authored such that the first thing the VM does upon running it is execute a self-test. After ironing out some bugs and running the provided binary again the third code was printed to the console:
```self-test complete, all tests pass
The self-test completion code is: BNCyODLfQkIl
```

## Code #4: A Text-Based RPG?
After the self-test finished executing, I was surprised to find myself playing a text-based RPG. A short interaction was enough to gain my fourth code:
```
== Foothills ==
You find yourself standing at the base of an enormous mountain.  At its base to the north, there is a massive doorway.
A sign nearby reads "Keep out!  Definitely no treasure within!"

Things of interest here:
- tablet

There are 2 exits:
- doorway
- south

What do you do?
take tablet

Taken.

What do you do?
use tablet

You find yourself writing "pWDWTEfURAdS" on the tablet.  Perhaps it's some kind of code?
```

## Code #5: Time To Go Spelunking!
Playing the text-based RPG lead me into a circuitous cave system with caverns whose names were all permutations of the same couple words:
```
You are in a maze of twisty little passages, all alike.
You are in a twisty alike of little passages, all maze.
You are in a little maze of twisty passages, all alike.
```

I considered writing some path-finding code to explore the maze, but not knowing exactly what I was looking for I proceeded to play manually. As I played, I constructed the below map, which allowed me to find a lantern, oil for the lantern, and a __*code*__ chiseled onto one of the cave walls:

![synacor_cave_map](https://github.com/tmbarker/synacor-challenge/assets/50631648/c1b7de3e-b2fd-48f5-a7a2-5783a2f3e291)

## Code #6: Exploration, Arithmetic, and Teleportation
After emerging from the cave system I encountered a set of ruins:
```
== Ruins ==
You stand in the massive central hall of these ruins.  The walls are crumbling, and vegetation has clearly taken over.
Rooms are attached in all directions.  There is a strange monument in the center of the hall with circular slots and
unusual symbols.  It reads:
_ + _ * _^2 + _^3 - _ = 399
```

Exploring the adjacent areas yielded 5 coins, each of which, upon inspection, had some sort of numerical marking:
```
--look red coin
This coin is made of a red metal.  It has two dots on one side.

--look corroded coin
This coin is somewhat corroded.  It has a triangle on one side.

--look shiny coin
This coin is somehow still quite shiny.  It has a pentagon on one side.

--look concave coin
This coin is slightly rounded, almost like a tiny bowl.  It has seven dots on one side.

--look blue coin
This coin is made of a blue metal.  It has nine dots on one side.
```

After solving which permutation of these coins would satisfy the above equation, I inserted them into the slots in the monument:
```
As you place the last coin, you hear a click from the north door.
```
In the next area I found a teleporter. Using it transported me directly to Synacor HQ, but not before giving me a glimpse of a __*code*__ in the stars:
```
You activate the teleporter!  As you spiral through time and space, you think you see a pattern in the stars...
    JyDQhSbkpyns
```

## Code #7: Teleportation, Disassembly, and a Man Named Wilhelm Ackermann
After the previous activation of the teleporter I found myself at Synacor HQ. A nearby book contained a lengthy passage about teleportation, and it was clear what I had to do to acquire my next code:
1. The teleporter will transport you to one of two locations when you use it, based on the value in the eighth register
2. If the value in the eighth register is zero, you are transported to Synacor HQ
3. If the value in the eighth register is non-zero, it is first verified by a "long running confirmation mechanism", and if valid you are transported to a different (and unknown) location
4. The confirmation mechanism will take "A billion years" to execute naively, so it must be reverse engineered and optimized

The first thing I did was disassemble the challenge binary to a text file. Luckily, there are only a few usages of register 8 in the challenge binary. It became clear that the confirmation mechanism was invoked as follows, with the block of code at IP 6049 being the confirmation mechanism itself:
```
5505:   set  reg[0]       4
5508:   set  reg[1]       1
5511:  call    6049
5513:    eq  reg[1]  reg[0]       6
```

Here is the "raw assembly" of "function 6049":
```
6049:    jt  reg[0]    6057
6052:   add  reg[0]  reg[1]       1
6056:   ret
6057:    jt  reg[1]    6070
6060:   add  reg[0]  reg[0]   32767
6064:   set  reg[1]  reg[7]
6067:  call    6049
6069:   ret
6070:  push  reg[0]
6072:   add  reg[1]  reg[1]   32767
6076:  call    6049
6078:   set  reg[1]  reg[0]
6081:   pop  reg[0]
6083:   add  reg[0]  reg[0]   32767
6087:  call    6049
6089:   ret
```

This chunk of code is only 16 lines long, but it took me an extremely long time to make sense of it. One important piece is the following:
```
60XX:   add  reg[X]  reg[X]   32767
```
Because all arithmetic in the Synacor VM is modulo `32768`, these lines implement simple subtraction of `1`, or equivalently a decrement operation.

One spoiler I had going into this challenge was that "Ackermann" was involved in some way. I previously had no idea what "Ackermann" was, but I learned that he was a computer scientist, given name Wilhelm, whose work on total computable functions famously yielded the "Ackermann Function". The Ackermann Function has many forms, but all of them characteristically exhibit explosive growth in value.

It turns out that function 6049 is a slightly modified version of the 2-ary Ackermann function. Translated to C#, it looks like this:
```cs
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
```

In the above snippet `Inc` and `Dec` are simple increment and decrement operators that respect the system wide modulo constraint, and `x` is the value in the eighth register.

The confirmation mechanism is validating that after function 6049 is invoked that the first register contains the value `6`. This means I need to find the value of `x` (register 8) that makes function 6049 return `6`. I found this value by linearly checking every value between `1` and the system modulus, and found it to be `25734`. One hiccup was due to the recursion depth, even with memoization, I had to run this search in a new thread with a large stack size limit to avoid stack overflow.

Putting it all together, I was able to acquire my 7th code:
```
set-breakpoint 5500
set-reg 7 25734
run
set-reg 0 6
set-ip 5513
run

You wake up on a sandy beach with a slight headache.  The last thing you remember is activating that teleporter... but now you can't find it anywhere in your pack.  Someone seems to have drawn a message in the sand here:

    NBlOWKLbTMgY

It begins to rain.  The message washes away.
```

## Code #8: A Tropical Heist
After being teleported to a tropical beach some simple exploration lead to a journal which succinctly described the final puzzle to be solved. A four-by-four array of rooms separated an "Orb", and a "Vault". The rooms were set up like a checkerboard, with each "square" of one parity containing an arithmetic operation (`+`, `-`, or `*`), and each of the other parity containing a numeric literal. The goal was to pick up the orb in the first room and navigate to the vault such that the cumulative score of the path taken matched the value written on the vault door.

![orbs_map](https://github.com/tmbarker/synacor-challenge/assets/50631648/9fb9809a-61b8-4397-8409-7875b267fd43)

This was a path-finding problem, so the first thing I did was whip up some utilities to make solving it easier:
```cs
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
```

Next, I implemented a standard queue based Breadth First Search, iterating two spatial steps at a time, where the first step was into an operator room, and the second was into a numeric literal room. The core of my BFS looked like this:
```cs
foreach (var step1 in Vector2D.Dirs)
foreach (var step2 in Vector2D.Dirs)
{
    var operatorPos = state.Pos + step1;
    var literalPos  = operatorPos + step2;

    // Some edge cases here removed for brevity

    var adjacent = new State(Pos: literalPos, Weight: map.Operators[operatorPos] switch
    {
        "+" => state.Weight + map.Literals[literalPos],
        "-" => state.Weight - map.Literals[literalPos],
        "*" => state.Weight * map.Literals[literalPos]
    });
    
    if (visited.Add(adjacent))
    {
        queue.Enqueue(adjacent);
        paths[adjacent] = BuildPath(paths[state], step1, step2);
    }
}
```
Running my code yielded this path:

![synacor_orb_solution_annotated](https://github.com/user-attachments/assets/01f3e21c-381e-4abb-adcb-61d23f6715ec)

The only thing left to do was to let my pathfinding code navigate for me, and then enter the vault:
```
Synacor Shell started.
- Type 'help' for cmd listing
- Type 'quit' to quit
- Commands prepended with '--' are passed thru to the VM instance input buffer

load C:\...\orb.syn  
VM state loaded from C:\...\orb.syn
solve-orb

[Automated pathfinding output removed]

== Vault Door ==
You stand before the door to the vault; it has a large '30' carved into it.  Affixed to the wall near the door, there is a running hourglass which never seems to run out of sand.

The floor of this room is a large mosaic depicting the number '1'.

What do you do?
--vault

== Vault ==
This vault contains incredible riches!  Piles of gold and platinum coins surround you, and the walls are adorned with topazes, rubies, sapphires, emeralds, opals, dilithium crystals, elerium-115, and unobtainium.

Things of interest here:
- mirror

What do you do?
--take mirror

Taken.

What do you do?
--use mirror

You gaze into the mirror, and you see yourself gazing back.  But wait!  It looks like someone wrote on your face while you were unconscious on the beach!  Through the mirror, you see "iW8UwOHpH8op" scrawled in charcoal on your forehead.

Congratulations; you have reached the end of the challenge!
```

## Summary
This was a really fun challenge, even though I'm very late to the party I'm happy to have solved it. In my opinion code 7 was by far the most difficult part of the challenge; even though I've been doing more and more disassembly problems they do not come easy to me. 

Thanks to Eric Wastl for issuing the challenge! For anyone reading this feel free to reach out if you have any questions about this repository.
