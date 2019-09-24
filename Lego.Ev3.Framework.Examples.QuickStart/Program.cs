using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
namespace Lego.Ev3.Framework.Examples.QuickStart
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile("brick.json", optional: true, reloadOnChange: true); // set the file to copy if newer

            IConfigurationRoot configuration = builder.Build();

            ServiceProvider provider = new ServiceCollection()
           .AddBrick(configuration) //adds the brick as singleton instance to servicecollection
           .AddLogging(opt => opt.AddConsole().SetMinimumLevel(LogLevel.Trace))
           .AddTransient<Robot>()
           .BuildServiceProvider();

            Robot robot = provider.GetRequiredService<Robot>();
            robot.Start().GetAwaiter().GetResult();
            Console.ReadKey();
            robot.Stop().GetAwaiter().GetResult();
            Console.ReadKey();
        }
    }
}
