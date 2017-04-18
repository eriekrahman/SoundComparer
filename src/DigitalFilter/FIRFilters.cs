using System;

namespace SoundComparer.DigitalFilter
{
    /// <summary>
    /// Helper class which implements the filtering functions. Currently Low Pass, High Pass and
    /// Band Pass are implemented.
    /// </summary>
    class FIRFilters
    {
        #region Members

        /// <summary>
        /// The number of samples is the same as the number of points
        /// </summary>
        private int mySamples;

        /// <summary>
        /// Holds the coefficient from the window function
        /// </summary>
        private float[] myCoeff;

        /// <summary>
        /// Holds the series which has the input data
        /// </summary>
        private float[] myInputSeries;

        /// <summary>
        /// FFT algorithm object
        /// </summary>
        private FFT myFFT;

        /// <summary>
        /// Holds the current algorithm selected. Enumeration type is drawn from the FFT object.
        /// </summary>
        public FFT.Algorithm CurrentAlgorithm;
        public FFT.FilterType CurrentFilter;
        private float myFreqFrom;
        private float myFreqTo;
        private float myAttenuation;
        private float myBand;
        private float myAlpha;
        private int myTaps;
        private int myOrder;

        #endregion // Members

        #region Properties

        /// <summary>
        /// The starting passband frequency, must be lower than ending frequency.
        /// </summary>
        public float FreqFrom
        {
            get { return myFreqFrom; }
            set { myFreqFrom = value; }
        }

        /// <summary>
        /// The ending passband frequency, must be higher than starting frequency.
        /// </summary>
        public float FreqTo
        {
            get { return myFreqTo; }
            set { myFreqTo = value; }
        }

        /// <summary>
        /// Stopband attenuation
        /// </summary>
        public float StopBandAttenuation
        {
            get { return myAttenuation; }
            set {
                    myAttenuation = value;
                    this.myFFT.StopBandAttenuation = myAttenuation;
                }
        }

        /// <summary>
        /// Transition band
        /// </summary>
        public float TransitionBand
        {
            get { return myBand; }
            set {
                    myBand = value;
                    this.myFFT.TransitionBand = myBand;
                }
        }

        /// <summary>
        /// Alpha value used for the Kaiser algorithm.
        /// </summary>
        public float Alpha
        {
            get { return myAlpha; }
            set {
                    myAlpha = value;
                    this.myFFT.Alpha = myAlpha;
                }
        }

        /// <summary>
        /// Number of taps to be used. Taps is the number of samples processed at any one time.
        /// </summary>
        public int Taps
        {
            get { return myTaps; }
            set { myTaps = value; }
        }

        /// <summary>
        /// Filter order. Must be an even number.
        /// </summary>
        public int Order
        {
            get { return myOrder; }
            set
            {
                // Assure value is even
                if ((value % 2) == 0)
                {
                    myOrder = value;
                    this.myFFT.Order = myOrder;
                }
                else
                    throw new ArgumentOutOfRangeException("Order", "Filter order must be an even number.");
            }
        }

        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Main constructor. Resets all settings within the FFT algorithm object.
        /// </summary>
        public FIRFilters()
        {
            // Create a new FFT object
            this.myFFT = new FFT();

            // Default algorithm to Kaiser
            this.CurrentAlgorithm = FFT.Algorithm.Kaiser;

            // Default taps to 35
            this.myTaps = 35;
        }

        #endregion // Constructors

        #region Methods

        /// <summary>
        /// Performs a low pass filter. Output series will be cleared before being
        /// output to. If passband start and end frequencies are left at 0, defaults are used.
        /// </summary>
        public void LowPassFilter(ref float[] iseries)
        {
            // If no start and end frequencies are specified, default low pass frequency range to:
            // 0 - 1000hz
            if (this.myFreqFrom == 0.0f && this.myFreqTo == 0.0f)
            {
                this.myFFT.FreqFrom = 0.0f;
                this.myFFT.FreqTo = 1000.0f;
            }
            else
            {
                this.myFFT.FreqFrom = this.myFreqFrom;
                this.myFFT.FreqTo = this.myFreqTo;
            }

            // Filter the series based on the coefficients generated
            Filter(ref iseries);
        }

        public void CalculateCoefficients(FFT.Algorithm algorithm, FFT.FilterType filter)
        {
            // Generate the actual coefficients
            this.CurrentAlgorithm = algorithm;
            this.CurrentFilter = filter;
            if (algorithm == FFT.Algorithm.Kaiser)
            {
                
                // For kaiser window function we need to set the attentuation
                this.StopBandAttenuation = 60;

                // Kaiser also requires transition band
                this.TransitionBand = 500;
            }

            if (this.myFreqFrom == 0.0f && this.myFreqTo == 0.0f)
            {
                this.myFFT.FreqFrom = 1000;
                this.myFFT.FreqTo = 1000f;
            }
            else
            {
                this.myFFT.FreqFrom = this.myFreqFrom;
                this.myFFT.FreqTo = this.myFreqTo;
            }
   
            this.myCoeff = this.myFFT.GenerateCoefficients(this.CurrentFilter,this.CurrentAlgorithm);
        }

        /// <summary>
        /// Performs a high pass filter. Output series will be cleared before being
        /// output to. If passband start and end frequencies are left at 0, defaults are used.
        /// </summary>
        public void HighPassFilter(ref float[] iseries)
        {
            // If no start and end frequencies are specified, default high pass frequency range to:
            // 2000 - 4000hz
            if (this.myFreqFrom == 0.0f && this.myFreqTo == 0.0f)
            {
                this.myFFT.FreqFrom = 2000.0f;
                this.myFFT.FreqTo = 4000.0f;
            }
            else
            {
                this.myFFT.FreqFrom = this.myFreqFrom;
                this.myFFT.FreqTo = this.myFreqTo;
            }

            // Filter the series based on the coefficients generated
            Filter(ref iseries);
        }

        /// <summary>
        /// Performs a band pass filter. Output series will be cleared before being
        /// output to. If passband start and end frequencies are left at 0, defaults are used.
        /// </summary>
        public void BandPassFilter(ref float[] iseries)
        {
            // If no start and end frequencies are specified, default band pass frequency range to:
            // 1000 - 1000hz
            if (this.myFreqFrom == 0.0f && this.myFreqTo == 0.0f)
            {
                this.myFFT.FreqFrom = 1000;
                this.myFFT.FreqTo = 1000f;
            }
            else
            {
                this.myFFT.FreqFrom = this.myFreqFrom;
                this.myFFT.FreqTo = this.myFreqTo;
            }

            // Filter the series based on the coefficients generated
            Filter(ref iseries);
        }

        #endregion // Methods

        #region Initialization

        /// <summary>
        /// Initializes the FIRFilters object by setting the input and output series members for use
        /// by the filter.
        /// </summary>
        /// <param name="iseries">Input series that contains input data</param>
        /// <param name="oseries">Output series to which filter will be written</param>
        private void SetIOSeries(float[]  iseries)
        {
            this.myInputSeries = iseries;

            // Samples is the number of points contained in the input
            this.mySamples = myInputSeries.Length;
        }

        #endregion // Initialization

        #region Filter

        /// <summary>
        /// Performs the actual filter. Coefficients should have already be generated by the calling
        /// function, this function merely applies them and physically adds the points to the output series.
        /// </summary>
        private void Filter(ref float[] iseries)
        {
            float[] x = new float[myTaps];
            float y; 
            
            // Set the series
            SetIOSeries(iseries);

            // Initialize x
            for (int i = 1; i < myTaps; i++)
                x[i] = 0.0f;

            // Loop through every data point
            for (int i = 0; i < iseries.Length; i++)
            {
                // Initialize y
                y = 0.0f;

                // Obtain the data value (Y value) at the specified X value (i)
                x[0] = Convert.ToSingle(iseries[i]);

                // Loop through from 0 to number of taps and calculate the sum
                try
                {
                    for (int j = 0; j < myTaps; j++)
                        y = y + (x[j] * myCoeff[j]);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message + " Check filter order.");
                    throw e;
                }

                // Shift all x values by 1 to the right
                for (int j = myTaps - 1; j > 0; j--)
                    x[j] = x[j - 1];

                // Add the y value to the output series at the current x value
                iseries[i]=y;
            }
        }

        #endregion // Filter
    }
}
