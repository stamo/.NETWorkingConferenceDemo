using nanoFramework.Hardware.Esp32;
using System.Device.Gpio;

namespace WiFiRelay
{
    public static class PinController
    {
        private static GpioController gpioController;
        private static GpioPin firstRelay;
        private static GpioPin secondRelay;

        public static RelayState State { get; set; } = new RelayState();

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
