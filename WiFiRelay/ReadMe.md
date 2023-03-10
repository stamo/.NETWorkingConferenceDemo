# WiFi Relay

This project is using Xiao ESP32C3 development board with two relays and two light bulbs to demonstrate controll of lights over WiFi. 

## Needed hardware

1. Xiao ESP32C3 development board
2. xxx relay module
3. Two light bulbs

## Wiring diagram

![Wiring diagram](Images/wifirelays.png)

## Prerequisites

In order to run this project you will have to add **.secrets.cs** file with following content:

```c#

namespace AirQuality
{
    /// <summary>
    /// Project secrets, must be ignored from repo
    /// </summary>
    internal static class Secrets
    {
        /// <summary>
        /// WiFi name
        /// </summary>
        internal const string WiFiSsid = "Your_WiFi_SSID";

        /// <summary>
        /// WiFi password
        /// </summary>
        internal const string WiFiPassword = "Your_WiFi_Password";

        /// <summary>
        /// Server TLS Certifivate
        /// </summary>
        internal const string ServerCertificate = "Your_Server_Certificate";

        /// <summary>
        /// Server TLS Private Key
        /// </summary>
        internal const string ServerPrivateKey = "Your_Server_Certificate_Private_Key";

        /// <summary>
        /// Server TLS Private Key Password
        /// </summary>
        internal const string KeyPassword = "Your_Private_Key_Password";
    }
}

```
