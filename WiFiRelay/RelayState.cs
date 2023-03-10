namespace WiFiRelay
{
    /// <summary>
    /// State of both relays
    /// </summary>
    public class RelayState
    {
        /// <summary>
        /// First relay state
        /// OFF by default
        /// </summary>
        public string FirstRelay { get; set; } = "Off";

        /// <summary>
        /// Second relay state
        /// OFF by default
        /// </summary>
        public string SecondRelay { get; set; } = "Off";
    }
}
