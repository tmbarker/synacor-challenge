// See https://aka.ms/new-console-template for more information

using Synacor.Utilities;
using Synacor.VirtualMachine;

var program = BinaryProvider.Read();
var vm = Vm.Create(program);

vm.BufferCommands(new[]
{
    "doorway",
    "north",
    "north",
    "bridge",
    "continue",
    "down",
    "east",
    "take empty lantern",
    "west",
    "west",
    "passage",
    "ladder",
    "west",
    "south",
    "north",
    "take can",
    "use can",
    "west",
    "ladder",
    "darkness",
    "use lantern",
    "continue",
    "west",
    "west",
    "west",
    "west",
    "north",
});

vm.Run();