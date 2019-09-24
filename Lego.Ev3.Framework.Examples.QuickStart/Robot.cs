using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
namespace Lego.Ev3.Framework.Examples.QuickStart
{

    /*
     * Robot using dependency injection and json configuration see Program.cs
     * 
     * To Run:
     * 
     * Add a touch sensor to Port One
     * Add a large motor to Port A
     * 
     * CTRL + F5 Run
     * 
     * Motor will run until touchsensor is clicked or key is pressed in the console
     */
    public class Robot
    {
        private readonly Brick _brick;
        private readonly ILogger<Robot> _logger;

        private TouchSensor _touchSensor;

        private LargeMotor _largeMotor;

        public Robot(Brick brick, ILogger<Robot> logger)
        {
            _brick = brick;
            _logger = logger;
        }

        public async Task Start()
        {
            //first find all devices and wire up input device events before connecting
            _largeMotor = _brick.FindDevice<LargeMotor>("largeMotorId");
            _touchSensor = _brick.FindDevice<TouchSensor>("touchSensorId");
            _touchSensor.InputChanged += TouchSensor_InputChanged;

            // connect to the brick
            await _brick.Connect();

            //from here onward we can call any device method
            _logger.LogInformation("starting motor");
            await _largeMotor.Start();  

           
        }

        public async Task Stop()
        {
            //method will also stop all devices and finally disconnect the brick
            await _brick.Disconnect();
        }

        private async void TouchSensor_InputChanged(TouchSensor sensor, int value)
        {
            _logger.LogInformation($"TouchSensor {sensor.Id} input changed value:{value}");
            await _largeMotor.Stop();
        }
    }
}
