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
You find yourself standing at the base of an enormous mountain.  At its base to the north, there is a massive doorway.  A sign nearby reads "Keep out!  Definitely no treasure within!"

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

![synacor_cave_map](https://github.com/tmbarker/synacor-challenge/assets/50631648/5f153d6c-fdf7-4a95-ad0b-bd290dba3894)
