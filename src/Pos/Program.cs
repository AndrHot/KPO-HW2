using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Pos;
using Pos.Services;
using Pos.Ui;

Console.OutputEncoding = Console.InputEncoding = Encoding.UTF8;

using var provider = new ServiceCollection().AddPos().BuildServiceProvider();
var ui = new ConsoleUi(provider.GetRequiredService<Journal>(), Console.In, Console.Out);

if (args.Contains("--demo"))
    ui.RunDemo();
else
    ui.Run();
