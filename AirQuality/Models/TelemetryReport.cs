namespace AirQuality.Models
{
    /// <summary>
    /// Properties to be sent to Azure as telemetry data
    /// </summary>
    internal class TelemetryReport
    {
        /// <summary>
        /// Temperature value in degrees celsius
        /// </summary>
        public double Temperature { get; init; }

        /// <summary>
        /// Relative humidity value in percents
        /// </summary>
        public double Humidity { get; init; }

        /// <summary>
        /// Pressure value in hectopascals
        /// </summary>
        public double Pressure { get; init; }

        /// <summary>
        /// Dust partical (10 μm) concentration in µg/m3
        /// </summary>
        public double PM10 { get; set; }

        /// <summary>
        /// Dust partical (2.5 μm) concentration in µg/m3
        /// </summary>
        public double PM25 { get; set; }
    }
}
