using Lego.Ev3.Framework;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ColorSensorLogger
{
    public class Robot
    {
        private readonly Brick Brick;

        private readonly ILogger<Robot> Logger;

        private List<LogEntry> Log { get; } = new List<LogEntry>();

        public Robot(Brick brick, ILogger<Robot> logger)
        {
            Brick = brick;
            Logger = logger;

            ColorSensor colorSensor = brick.FindDevice<ColorSensor>("colorSensor");
            colorSensor.InputChanged += ColorSensor_InputChanged;
        }

        public async Task Start()
        {
            Log.Clear();
            await Brick.Connect();
        }

        public async Task Stop()
        {
            await Brick.Disconnect();

            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ColorSensorLog.csv");
            using (System.IO.StreamWriter writer = System.IO.File.CreateText(path)) 
            {
                foreach (LogEntry entry in Log) 
                {
                    writer.Write($"{entry.TimeStamp},{entry.ColorSensorId},{entry.Value},{Environment.NewLine}");
                }
                writer.Flush();
            }
            Log.Clear();
        }

        private void ColorSensor_InputChanged(ColorSensor sensor, ColorSensorValue value)
        {
            Logger.LogInformation($"{sensor.Id} {value.Value}");
            Log.Add(new LogEntry(sensor, value));
        }
    }


    public class LogEntry 
    {
        public string ColorSensorId { get; }

        public int Value { get; }

        public DateTime TimeStamp { get; }

        public LogEntry(ColorSensor sensor, ColorSensorValue value) 
        {
            ColorSensorId = sensor.Id;
            Value = value.Value;
            TimeStamp = DateTime.Now;
        }
    }
}
