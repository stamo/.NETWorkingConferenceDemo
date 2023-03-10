using nanoFramework.Hardware.Esp32;
using System.Device.Gpio;

namespace WiFiRelay
{
    /// <summary>
    /// Controlls GPIO pins
    /// </summary>
    public static class PinController
    {
        /// <summary>
        /// nanoFramework GPIO Controller
        /// </summary>
        private static GpioController gpioController;

        /// <summary>
        /// Controll pin of the first relay
        /// </summary>
        private static GpioPin firstRelay;

        /// <summary>
        /// Controll pin of the second relay
        /// </summary>
        private static GpioPin secondRelay;

        /// <summary>
        /// Both relays state
        /// </summary>
        public static RelayState State { get; set; } = new RelayState();

        /// <summary>
        /// First relay
        /// </summary>
        public static GpioPin FirstRelay
        { 
            get 
            {
                if (firstRelay == null)
                {
                    InitializeRelay();
                }

                return firstRelay;
            }
        }

        /// <summary>
        /// Second relay
        /// </summary>
        public static GpioPin SecondRelay
        {
            get
            {
                if (secondRelay == null)
                {
                    InitializeRelay();
                }

                return secondRelay;
            }
        }

        /// <summary>
        /// Initializes controlling pins
        /// Sets both states to OFF
        /// </summary>
        private static void InitializeRelay()
        {
            gpioController = new GpioController();
            firstRelay = gpioController.OpenPin(Gpio.IO02, PinMode.Output);
            secondRelay = gpioController.OpenPin(Gpio.IO03, PinMode.Output);
            firstRelay.Write(PinValue.Low);
            secondRelay.Write(PinValue.Low);
        }
    }
}
