// See https://aka.ms/new-console-template for more information

using Synacor.Utilities;
using Synacor.VirtualMachine;

var program = BinaryProvider.Read();
var vm = Vm.Create(program);

vm.Run();