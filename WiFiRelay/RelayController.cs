using nanoFramework.Json;
using nanoFramework.WebServer;
using System.Device.Gpio;
using System.Net;
using System.Text;

namespace WiFiRelay
{
    /// <summary>
    /// HTTP request handler
    /// </summary>
    public class RelayController
    {
        /// <summary>
        /// Block other treads from using GPIO
        /// </summary>
        private static readonly object _lock = new();

        /// <summary>
        /// relays endpoint handler
        /// returns state of both relays
        /// </summary>
        /// <param name="e">Request parameters</param>
        [Route("relays")]
        public void Get(WebServerEventArgs e)
        {
            string ret = JsonConvert.SerializeObject(PinController.State);
            
            e.Context.Response.ContentType = "application/json";
            e.Context.Response.ContentLength64 = ret.Length;

            WebServer.OutPutStream(e.Context.Response, ret);
        }

        /// <summary>
        /// Change state to ON
        /// </summary>
        /// <param name="e">Request parameters</param>
        [Route("relay/on")]
        public void PutOn(WebServerEventArgs e)
        {
            SetPinValue(e, PinValue.High);
        }

        /// <summary>
        /// Change state to OFF
        /// </summary>
        /// <param name="e">Request parameters</param>
        [Route("relay/off")]
        public void PutOff(WebServerEventArgs e)
        {
            SetPinValue(e, PinValue.Low);
        }

        /// <summary>
        /// Sets GPIO value
        /// </summary>
        /// <param name="e">Request parameters</param>
        /// <param name="pinValue">Value to set</param>
        private void SetPinValue(WebServerEventArgs e, PinValue pinValue)
        {
            bool requestIsValid = false;

            lock (_lock) 
            {
                var parameters = GetParameters(e);

                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        if (param.Name.ToLower() == "n")
                        {
                            requestIsValid = true;

                            switch (param.Value)
                            {
                                case "1":
                                    PinController.FirstRelay.Write(pinValue);
                                    PinController.State.FirstRelay = pinValue == PinValue.High ? "On" : "Off";

                                    WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.OK);
                                    break;
                                case "2":
                                    PinController.SecondRelay.Write(pinValue);
                                    PinController.State.SecondRelay = pinValue == PinValue.High ? "On" : "Off";

                                    WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.OK);
                                    break;
                                default:
                                    WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.NotFound);
                                    break;
                            }

                            break;
                        }
                    }
                }

                if (requestIsValid == false)
                {
                    WebServer.OutputHttpCode(e.Context.Response, HttpStatusCode.BadRequest);
                }
            }
        }

        /// <summary>
        /// Get parameters from request
        /// </summary>
        /// <param name="e">Request parameters</param>
        /// <returns></returns>
        private UrlParameter[] GetParameters(WebServerEventArgs e)
        {
            var parameters = WebServer.DecodeParam(e.Context.Request.RawUrl);

            if (parameters == null && e.Context.Request.ContentLength64 > 0)
            {
                byte[] buff = new byte[e.Context.Request.ContentLength64];
                e.Context.Request.InputStream.Read(buff, 0, buff.Length);
                string rawData = WebServer.ParamStart.ToString();
                rawData += new string(Encoding.UTF8.GetChars(buff));

                parameters = WebServer.DecodeParam(rawData);
            }

            return parameters;
        }
    }
}
