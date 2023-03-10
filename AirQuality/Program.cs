using AirQuality.Models;
using Iot.Device.Bmxx80;
using nanoFramework.Azure.Devices.Client;
using nanoFramework.Azure.Devices.Shared;
using nanoFramework.Hardware.Esp32;
using nanoFramework.Json;
using nanoFramework.Networking;
using System;
using System.Device.I2c;
using System.Diagnostics;
using System.IO.Ports;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace AirQuality
{
    /// <summary>
    /// Air quality measurments
    /// Measure temperature, humidity, pressure 
    /// and dust particals concentration 
    /// and send them to Azure IoT hub
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Serial device used to comunicate with 
        /// SDS011 dust partical sensor
        /// </summary>
        private static SerialPort _sds011;

        /// <summary>
        /// Circular buffer for Pm25 dust particals concentraition
        /// </summary>
        private static CircularBuffer _sdcPm25Buffer = new CircularBuffer(20);

        /// <summary>
        /// Circular buffer for Pm10 dust particals concentraition
        /// </summary>
        private static CircularBuffer _sdcPm10Buffer = new CircularBuffer(20);

        /// <summary>
        /// Azure IoT hub device client
        /// </summary>
        private static DeviceClient _azureIoT;

        public static void Main()
        {
            // Trying to connect to WiFi,
            // will retry every 10 seconds if not succeeded
            ConnectWifi();

            // Create Azure IoT device client
            _azureIoT = new DeviceClient(
                Secrets.IotBrokerAddress,
                Secrets.DeviceID,
                Secrets.SasKey,
                azureCert: new X509Certificate(Resource.GetBytes(Resource.BinaryResources.AzureRoot)));

            // Set status updated event handler
            _azureIoT.StatusUpdated += StatusUpdatedEvent;

            // Connects to Azure IoT hub
            ConnectAzureIoT();

            // Initializing ports and sensors
            InitializeDevice();

            while (true) 
            {
                // Sends telemetry to Azure
                SendTelemetry();

                Thread.Sleep(30000);
            }
        }

        /// <summary>
        /// Sends telemetry to Azure
        /// </summary>
        private static void SendTelemetry()
        {
            Bme280Sample bmeData = PerformBme280Sampling();
            TelemetryReport report = new TelemetryReport()
            {
                Humidity = bmeData.Humidity,
                PM10 = _sdcPm10Buffer.GetAverage(),
                PM25 = _sdcPm25Buffer.GetAverage(),
                Pressure = bmeData.Pressure,
                Temperature = bmeData.Temperature
            };

            _azureIoT.SendMessage(JsonSerializer.SerializeObject(report));
        }

        /// <summary>
        /// Connects to Azure IoT hub
        /// </summary>
        private static void ConnectAzureIoT()
        {
            bool isOpen = false;

            while (isOpen == false) 
            {
                isOpen = _azureIoT.Open();

                if (!isOpen)
                {
                    _azureIoT.Close();
                    Debug.WriteLine("Trying to reconnect to Azure IoT. Please check that all the credentials are correct.");

                    Thread.Sleep(10000);
                }
            }

            Debug.WriteLine($"Connection is open");
        }

        /// <summary>
        /// Trying to connect to WiFi,
        /// will retry every 10 seconds if not succeeded  
        /// </summary>
        private static void ConnectWifi()
        {
            bool isConnected = false;

            while (isConnected == false)
            {
                CancellationTokenSource cs = new(60000);
                isConnected = WifiNetworkHelper.ConnectDhcp(Secrets.WiFiSsid, Secrets.WiFiPassword, requiresDateTime: true, token: cs.Token);

                if (!isConnected)
                {
                    Debug.WriteLine($"Can't get a proper IP address and DateTime, error: {WifiNetworkHelper.Status}.");
                    if (WifiNetworkHelper.HelperException != null)
                    {
                        Debug.WriteLine($"Exception: {WifiNetworkHelper.HelperException}");
                    }

                    Thread.Sleep(10000);
                }
            }
        }

        /// <summary>
        /// Initializing ports and sensors
        /// </summary>
        private static void InitializeDevice()
        {
            // Configuring serial port
            Configuration.SetPinFunction(32, DeviceFunction.COM2_RX);
            Configuration.SetPinFunction(33, DeviceFunction.COM2_TX);

            // Configuring I2C
            Configuration.SetPinFunction(21, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(22, DeviceFunction.I2C1_CLOCK);

            // create and configure COM2
            _sds011 = new SerialPort("COM2")
            {
                BaudRate = 9600,
                Parity = Parity.None,
                StopBits = StopBits.One,
                Handshake = Handshake.None,
                DataBits = 8,
                ReadBufferSize = 2048
            };

            // open the serial port with the above settings
            _sds011.Open();

            // Set a watch for the return character
            _sds011.WatchChar = (char)0xAB;

            // register for the data received event
            _sds011.DataReceived += _sds011_DataReceived;

            // Set reported parameters
            TwinCollection reported = new TwinCollection();
            reported.Add("firmware", "nfAirQuality");
            reported.Add("version", 1.0);
            _azureIoT.UpdateReportedProperties(reported);
        }

        /// <summary>
        /// SDS011 data received event
        /// </summary>
        /// <param name="sender">SDS011 instance</param>
        /// <param name="e">Received Event Argumentss</param>
        private static void _sds011_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType == SerialData.Chars)
            {
                // Ignore the in between noise
            }
            else if (e.EventType == SerialData.WatchChar)
            {
                SerialPort serDev = (SerialPort)sender;

                if (serDev.BytesToRead > 0)
                {
                    byte[] rawData = new byte[serDev.BytesToRead];

                    var bytesRead = serDev.Read(rawData, 0, rawData.Length);

                    if (bytesRead > 0)
                    {
                        // If rawData.Length == 10 and rawData[0] = 0xAA and rawData[9] = 0xAB and rawData[1] = 0xC0
                        // this means we have a valid measure package from the sensor
                        // and byte[2] = low byte, byte[3] = high byte of uint representing the AQI for PM 2.5
                        if (rawData.Length >= 10)
                        {
                            if ((rawData[0] == 0xAA) && (rawData[1] == 0xC0) && (rawData[9] == 0xAB))
                            {
                                // Need to do checksum
                                byte crc = 0;
                                for (int i = 0; i < 6; i++)
                                {
                                    crc += rawData[i + 2];
                                }
                                if (crc == rawData[8])
                                {
                                    // All right, we have a go !!!!
                                    double pm25 = 0, pm10 = 0;

                                    pm25 = (double)((int)rawData[2] | (int)(rawData[3] << 8)) / 10;
                                    pm10 = (double)((int)rawData[4] | (int)(rawData[5] << 8)) / 10;

                                    _sdcPm10Buffer.Add(pm10);
                                    _sdcPm25Buffer.Add(pm25);

                                    Debug.WriteLine(String.Format("Air quality index: {0}\tPM 10\t{1} µg / m3\tPM 2.5\t{2} µg / m3\tSensor: {3}",
                                        DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'\t'HH':'mm':'ss"),
                                        pm10.ToString("N1"),
                                        pm25.ToString("N1"),
                                        "SDS011"));
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Performs data gathering from Bme280
        /// </summary>
        private static Bme280Sample PerformBme280Sampling()
        {
            // Set I2C bus
            const int busId = 1;

            // I2C configuration
            I2cConnectionSettings i2cSettings = new(busId, Bme280.SecondaryI2cAddress, I2cBusSpeed.StandardMode);

            // Create I2C device
            using I2cDevice i2cDevice = I2cDevice.Create(i2cSettings);

            // Create Bme280 device 
            using Bme280 bme80 = new Bme280(i2cDevice)
            {
                TemperatureSampling = Sampling.Standard,
                PressureSampling = Sampling.Standard,
                HumiditySampling = Sampling.Standard
            };

            // Perform a synchronous measurement
            var readResult = bme80.Read();

            Debug.WriteLine(string.Format("Temperature: {0:F2}\u00B0C", readResult.Temperature.DegreesCelsius));
            Debug.WriteLine(string.Format("Pressure: {0:F2}hPa", readResult.Pressure.Hectopascals));
            Debug.WriteLine(string.Format("Relative humidity: {0:F2}%", readResult.Humidity.Percent));

            return new Bme280Sample(
                readResult.Temperature.DegreesCelsius, 
                readResult.Humidity.Percent, 
                readResult.Pressure.Hectopascals);
        }

        /// <summary>
        /// Asure IoT status updated
        /// </summary>
        /// <param name="sender">Event source</param>
        /// <param name="e">Event arguments</param>
        private static void StatusUpdatedEvent(object sender, StatusUpdatedEventArgs e)
        {
            Debug.WriteLine($"Status changed: {e.IoTHubStatus.Status}, {e.IoTHubStatus.Message}");

            if (e.IoTHubStatus.Status == Status.Disconnected)
            {
                Debug.WriteLine("Being disconnected from Azure IoT");
                // Trying to reconnect
                ConnectAzureIoT();
            }
        }
    }
}
