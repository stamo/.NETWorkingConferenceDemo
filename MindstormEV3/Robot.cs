﻿using Lego.Mindstorms;

namespace MindstormEV3
{
    /// <summary>
    /// Robot controller
    /// </summary>
    internal class Robot : IDisposable
    {
        /// <summary>
        /// Mindstorm client
        /// </summary>
        private readonly MindstormsClient<BluetoothCommunication> client;

        /// <summary>
        /// Communication channel
        /// </summary>
        private readonly BluetoothCommunication communication;

        /// <summary>
        /// Pause between actions 
        /// in order to avoid the impact of inertial forces
        /// </summary>
        private readonly TimeSpan pause = TimeSpan.FromMilliseconds(500);

        /// <summary>
        /// Creates new robot instance
        /// </summary>
        /// <param name="port">Bluetooth port</param>
        /// <param name="pause">Pause between actions in order to avoid the impact of inertial forces</param>
        public Robot(string port, TimeSpan? pause = null)
        {
            communication = new BluetoothCommunication(port);
            client = new MindstormsClient<BluetoothCommunication>(communication);

            if (pause != null && pause.HasValue)
            {
                this.pause = pause.Value;
            }

            client.ConnectAsync().Wait();
        }

        /// <summary>
        /// Disposes client and communication channel
        /// </summary>
        public void Dispose()
        {
            client.Dispose();
            communication.Dispose();
        }

        /// <summary>
        /// Moves robot forward
        /// </summary>
        /// <param name="distance">Distance in cm</param>
        /// <returns></returns>
        public Robot Forward(int distance)
        {
            client.SetMotorPolarityAsync(OutputPort.B, Polarity.Forward).Wait();
            client.SetMotorPolarityAsync(OutputPort.C, Polarity.Forward).Wait();
            uint time = (uint)(distance * 50);
            client.TurnMotorAtPowerForTimeAsync(OutputPort.B, 100, time, true).Wait();
            client.TurnMotorAtPowerForTimeAsync(OutputPort.C, 100, time, true).Wait();

            Thread.Sleep((distance * 50) + pause.Milliseconds);

            return this;
        }

        /// <summary>
        /// Moves robot backward
        /// </summary>
        /// <param name="distance">Distance in cm</param>
        /// <returns></returns>
        public Robot Backward(int distance)
        {
            client.SetMotorPolarityAsync(OutputPort.B, Polarity.Backward).Wait();
            client.SetMotorPolarityAsync(OutputPort.C, Polarity.Backward).Wait();
            uint time = (uint)(distance * 50);
            client.TurnMotorAtPowerForTimeAsync(OutputPort.B, 100, time, true).Wait();
            client.TurnMotorAtPowerForTimeAsync(OutputPort.C, 100, time, true).Wait();

            Thread.Sleep((distance * 50) + pause.Milliseconds);

            return this;
        }

        /// <summary>
        /// Turns robot left
        /// </summary>
        /// <returns></returns>
        public Robot TurnLeft()
        {
            client.SetMotorPolarityAsync(OutputPort.C, Polarity.Backward).Wait();
            client.SetMotorPolarityAsync(OutputPort.B, Polarity.Forward).Wait();
            client.TurnMotorAtPowerForTimeAsync(OutputPort.B, 100, 700, true).Wait();
            client.TurnMotorAtPowerForTimeAsync(OutputPort.C, 100, 700, true).Wait();

            Thread.Sleep(700 + pause.Milliseconds);

            return this;
        }

        /// <summary>
        /// Turns robot right
        /// </summary>
        /// <returns></returns>
        public Robot TurnRight()
        {
            client.SetMotorPolarityAsync(OutputPort.C, Polarity.Forward).Wait();
            client.SetMotorPolarityAsync(OutputPort.B, Polarity.Backward).Wait();
            client.TurnMotorAtPowerForTimeAsync(OutputPort.B, 100, 700, true).Wait();
            client.TurnMotorAtPowerForTimeAsync(OutputPort.C, 100, 700, true).Wait();

            Thread.Sleep(700 + pause.Milliseconds);

            return this;
        }

        /// <summary>
        /// Sings a Star Wars song
        /// </summary>
        /// <returns></returns>
        public Robot Sing()
        {
            //client.PlaySoundAsync(100, "Hello");
            var song = new StarWarsSong(client);
            song.Play().Wait();

            return this;
        }

        /// <summary>
        /// Shoots a cannon
        /// </summary>
        /// <returns></returns>
        public Robot Shoot()
        {
            client.SetMotorPolarityAsync(OutputPort.A, Polarity.Forward).Wait();
            client.TurnMotorAtPowerForTimeAsync(OutputPort.A, 100, 1000, true).Wait();

            Thread.Sleep(1000);

            return this;
        }
    }
}
