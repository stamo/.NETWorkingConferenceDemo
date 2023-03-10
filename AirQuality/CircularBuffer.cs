namespace AirQuality
{
    /// <summary>
    /// Circular buffer to get average measurments
    /// </summary>
    internal class CircularBuffer
    {
        private int _currentElement;

        private double[] _internalBuffer;

        private int _length;

        /// <summary>
        /// Buffer length
        /// </summary>
        public int Length 
        { 
            get 
            {
                return _length;
            } 
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="i">Index</param>
        /// <returns></returns>
        public double this[int i]
        {
            get 
            { 
                return _internalBuffer[i]; 
            }
        }

        /// <summary>
        /// Creates circular buffer 
        /// with default length of 10 elements
        /// </summary>
        public CircularBuffer()
        {
            _length = 10;
            _currentElement = -1;
            _internalBuffer = new double[10];

            for (int i = 0; i < _length; i++)
            {
                _internalBuffer[i] = double.MinValue;
            }
        }

        /// <summary>
        /// Creates circular buffer of desired length
        /// </summary>
        /// <param name="length">Buffer length</param>
        public CircularBuffer(int length)
        {
            _length = length;
            _currentElement = -1;
            _internalBuffer = new double[length];

            for (int i = 0; i < _length; i++)
            {
                _internalBuffer[i] = double.MinValue;
            }
        }

        /// <summary>
        /// Adds value to buffer
        /// </summary>
        /// <param name="value">Measurment value</param>
        internal void Add(double value) 
        {
            _currentElement++;

            if (_currentElement == _length)
            {
                _currentElement = 0;
            }

            _internalBuffer[_currentElement] = value;
        }

        /// <summary>
        /// Gets the average value from the buffer
        /// </summary>
        /// <returns></returns>
        internal double GetAverage()
        {
            double sum = 0;
            int count = 0;

            for (int i = 0; i < _length; i++)
            {
                count++;

                if (_internalBuffer[i] == double.MinValue)
                {
                    break;
                }

                sum += _internalBuffer[i];
            }

            return sum / count;
        }
    }
}
