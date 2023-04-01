using System;
using System.Diagnostics;
using System.Threading;
using nanoFramework.Networking;
using nanoFramework.WebServer;
using static NFWebServer.Resource;

namespace NFWebServer
{
    /// <summary>
    /// Web server serving my presentation
    /// </summary>
    public class Program
    {
        private static bool isConnected = false;

        public static void Main()
        {
            Debug.WriteLine("Hello from a webserver!");

            try
            {
                // Connect to WiFi network
                CancellationTokenSource cs = new(60000);
                isConnected = WifiNetworkHelper.ConnectDhcp(Secrets.WiFiSsid, Secrets.WiFiPassword, requiresDateTime: true, token: cs.Token);

                if (!isConnected)
                {
                    Debug.WriteLine($"Can't get a proper IP address and DateTime, error: {WifiNetworkHelper.Status}.");
                    if (WifiNetworkHelper.HelperException != null)
                    {
                        Debug.WriteLine($"Exception: {WifiNetworkHelper.HelperException}");
                    }

                    return;
                }

                // Create http web server on port 80
                using (WebServer server = new WebServer(80, HttpProtocol.Http))
                {
                    // set event handler
                    server.CommandReceived += ServerCommandReceived;

                    // Start the server.
                    server.Start();

                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex}");
            }
        }

        /// <summary>
        /// Event handler to handle web requests
        /// </summary>
        /// <param name="source">Event source</param>
        /// <param name="e">Event parameters</param>
        private static void ServerCommandReceived(object source, WebServerEventArgs e)
        {
            try
            {
                // Get URL
                var url = e.Context.Request.RawUrl;
                Debug.WriteLine($"Command received: {url}, Method: {e.Context.Request.HttpMethod}");

                // Actions to perferm
                if (url.ToLower() == "/health")
                {
                    // This is simple raw text returned
                    WebServer.OutPutStream(e.Context.Response, "Healthy");
                }
                else if(url.ToLower().IndexOf("/favicon.ico") == 0)
                {
                    // File response
                    WebServer.SendFileOverHTTP(e.Context.Response, "favicon.ico", Resource.GetBytes(Resource.BinaryResources.favicon));
                }
                else if(url.ToLower().IndexOf("/slides/") == 0)
                {
                    var routes = url.TrimStart('/').Split('/');
                    string resource = routes[1].Split('.')[0].ToLower();
                    byte[] svg = Resource.GetBytes(GetBinaryResouce(resource));

                    // File (Picture) response
                    WebServer.SendFileOverHTTP(e.Context.Response, routes[1], svg);
                }
                else
                {
                    var routes = url.TrimStart('/').Split('/');
                    string resource = string.Concat(routes[0].Split('.'));

                    // String (HTML) response
                    WebServer.OutPutStream(e.Context.Response, Resource.GetString(GetStringResouce(resource)));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex}");
            }
        }

        /// <summary>
        /// Get bunary resource from embeded resources
        /// </summary>
        /// <param name="resource">Resource name</param>
        /// <returns></returns>
        private static BinaryResources GetBinaryResouce(string resource)
        {
            switch (resource) 
            {
                case "slide1":
                    return BinaryResources.Slide1;
                case "slide2":
                    return BinaryResources.Slide2;
                case "slide3":
                    return BinaryResources.Slide3;
                case "slide4":
                    return BinaryResources.Slide4;
                case "slide5":
                    return BinaryResources.Slide5;
                case "slide6":
                    return BinaryResources.Slide6;
                case "slide7":
                    return BinaryResources.Slide7;
                case "slide8":
                    return BinaryResources.Slide8;
                case "slide9":
                    return BinaryResources.Slide9;
                case "slide10":
                    return BinaryResources.Slide10;
                case "slide11":
                    return BinaryResources.Slide11;
                case "slide12":
                    return BinaryResources.Slide12;
                case "slide13":
                    return BinaryResources.Slide13;
                case "slide14":
                    return BinaryResources.Slide14;
                case "slide15":
                    return BinaryResources.Slide15;
                case "slide16":
                    return BinaryResources.Slide16;
                case "slide17":
                    return BinaryResources.Slide17;
                case "slide18":
                    return BinaryResources.Slide18;
                default:
                    return BinaryResources.Slide1;
            }
        }

        /// <summary>
        /// Get string resource from embeded resources
        /// </summary>
        /// <param name="resource">Resource name</param>
        /// <returns></returns>
        private static StringResources GetStringResouce(string resource)
        {
            switch (resource)
            {
                case "slide1html":
                    return StringResources.slide1html;
                case "slide2html":
                    return StringResources.slide2html;
                case "slide3html":
                    return StringResources.slide3html;
                case "slide4html":
                    return StringResources.slide4html;
                case "slide5html":
                    return StringResources.slide5html;
                case "slide6html":
                    return StringResources.slide6html;
                case "slide7html":
                    return StringResources.slide7html;
                case "slide8html":
                    return StringResources.slide8html;
                case "slide9html":
                    return StringResources.slide9html;
                case "slide10html":
                    return StringResources.slide10html;
                case "slide11html":
                    return StringResources.slide11html;
                case "slide12html":
                    return StringResources.slide12html;
                case "slide13html":
                    return StringResources.slide13html;
                case "slide14html":
                    return StringResources.slide14html;
                case "slide15html":
                    return StringResources.slide15html;
                case "slide16html":
                    return StringResources.slide16html;
                case "slide17html":
                    return StringResources.slide17html;
                case "slide18html":
                    return StringResources.slide18html;
                default:
                    return StringResources.slide1html;
            }
        }
    }
}
