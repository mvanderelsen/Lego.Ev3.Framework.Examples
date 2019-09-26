using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
namespace Lego.Ev3.Framework.Examples.QuickStart
{
    class Program
    {
        private static Robot _robot;
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(ProcessExit);

            IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile("brick.json", optional: true, reloadOnChange: true); // set the file to copy if newer

            IConfigurationRoot configuration = builder.Build();

            ServiceProvider provider = new ServiceCollection()
           .AddBrick(configuration) //adds the brick as singleton instance to servicecollection
           .AddLogging(opt => opt.AddConsole().SetMinimumLevel(LogLevel.Trace))
           .AddTransient<Robot>()
           .BuildServiceProvider();

            _robot = provider.GetRequiredService<Robot>();
            _robot.Start().GetAwaiter().GetResult();
            Console.ReadKey();
        }

        private static void ProcessExit(object sender, EventArgs e)
        {
            _robot.Stop().GetAwaiter().GetResult();
            _robot = null;
        }
    }
}
