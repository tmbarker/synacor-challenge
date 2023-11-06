// See https://aka.ms/new-console-template for more information

using Synacor.Utilities;
using Synacor.VirtualMachine;

var vm = new Vm();
var program = BinaryProvider.Read();

vm.Load(program);
vm.Run();