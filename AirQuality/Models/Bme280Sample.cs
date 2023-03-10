namespace AirQuality.Models
{
    /// <summary>
    /// Sample data from Bme280
    /// </summary>
    internal class Bme280Sample
    {
        /// <summary>
        /// Creates sample data
        /// </summary>
        public Bme280Sample(double temperature, double humidity, double pressure)
        {
            Temperature = temperature;
            Humidity = humidity;
            Pressure = pressure;
        }

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
    }
}
