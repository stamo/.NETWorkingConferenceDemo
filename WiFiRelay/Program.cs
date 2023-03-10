using nanoFramework.Networking;
using nanoFramework.WebServer;
using System;
using System.Diagnostics;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace WiFiRelay
{
    public class Program
    {
        private static bool isConnected = false;

        public static void Main()
        {
            Debug.WriteLine("Hello from a webserver!");

            try
            {
                CancellationTokenSource cs = new(60000);
                isConnected = WifiNetworkHelper.ConnectDhcp(Secrets.WiFiSsid, Secrets.WiFiPassword, requiresDateTime: false, token: cs.Token);
                
                if (!isConnected)
                {
                    Debug.WriteLine($"Can't get a proper IP address and DateTime, error: {WifiNetworkHelper.Status}.");
                    if (WifiNetworkHelper.HelperException != null)
                    {
                        Debug.WriteLine($"Exception: {WifiNetworkHelper.HelperException}");
                    }

                    return;
                }

                byte[] cert = Convert.FromBase64String(Secrets.ServerCertificate);
                byte[] key = Convert.FromBase64String(Secrets.ServerPrivateKey);

                X509Certificate myWebServerCertificate509 = new X509Certificate2(cert, key, Secrets.KeyPassword);

                using (WebServer server = new WebServer(443, HttpProtocol.Https, new Type[] { typeof(RelayController) }))
                {
                    server.CommandReceived += ServerCommandReceived;
                    server.HttpsCert = myWebServerCertificate509;
                    server.SslProtocols = SslProtocols.Tls11 | SslProtocols.Tls12;

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

        private static void ServerCommandReceived(object source, WebServerEventArgs e)
        {
            try
            {
                var url = e.Context.Request.RawUrl;
                Debug.WriteLine($"Command received: {url}, Method: {e.Context.Request.HttpMethod}");

                if (url.ToLower() == "/health")
                {
                    // This is simple raw text returned
                    WebServer.OutPutStream(e.Context.Response, "Healthy");
                }
                else if (url.ToLower().IndexOf("/favicon.ico") == 0)
                {
                    WebServer.SendFileOverHTTP(e.Context.Response, "favicon.ico", Resource.GetBytes(Resource.BinaryResources.favicon));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex}");
            }
        }
    }
}
