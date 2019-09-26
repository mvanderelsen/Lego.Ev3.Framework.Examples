using Lego.Ev3.Framework;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
namespace QuickStart
{

    /*
     * Robot using dependency injection and json configuration see Program.cs
     * See README.md
     */
    public class Robot
    {
        public const int MINIMUM_BATTERY_LEVEL = 1;

        private readonly Brick _brick;

        private readonly ILogger<Robot> _logger;

        private readonly TouchSensor _touchSensor;

        private readonly LargeMotor _largeMotor;

        public Robot(Brick brick, ILogger<Robot> logger)
        {
            _brick = brick;
            _logger = logger;

            //first find all devices and wire up events before connecting
            _largeMotor = _brick.FindDevice<LargeMotor>("largeMotorId");
            _touchSensor = _brick.FindDevice<TouchSensor>("touchSensorId");
            _touchSensor.InputChanged += TouchSensor_InputChanged;

            _brick.Buttons.Up.Clicked += Button_Clicked;

            //monitor the battery level
            _brick.Battery.Mode = BatteryMode.Level;
            _brick.Battery.ValueChanged += Battery_ValueChanged;
        }


        public async Task Start()
        {
            // connect to the brick
            await _brick.Connect();

            //from here onward we can call any method

            _logger.LogInformation("Starting the robot");
            await _largeMotor.Run();            
        }


        public async Task Stop()
        {
            //method will also stop all devices and finally disconnect the brick
            await _brick.Disconnect();
        }


        private async void Battery_ValueChanged(BatteryValue value)
        {
            _logger.LogInformation($"Battery level:{value.Level}");
            if (value.Level < MINIMUM_BATTERY_LEVEL)
            {
                _logger.LogWarning("Fatal battery level aborting the program");
                await Stop();
            }
        }

        private async void Button_Clicked(Button button)
        {
            _logger.LogInformation($"Button {button.Type} clicked");
            if (_brick.Led.Mode == LedMode.OrangeFlashing)
            {
                _logger.LogInformation("Starting the robot");
                await _brick.Led.Reset();
                await _largeMotor.Run();
            }
            else _logger.LogWarning("Already running the robot");
        }

        private async void TouchSensor_InputChanged(TouchSensor sensor, int value)
        {
            _logger.LogInformation($"TouchSensor {sensor.Id} input changed value:{value}");
            await _largeMotor.Stop();
            _brick.Led.SetValue(LedMode.OrangeFlashing);
        }
    }
}
