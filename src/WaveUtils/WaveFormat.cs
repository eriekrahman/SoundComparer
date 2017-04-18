using System.Runtime.InteropServices;

namespace SoundComparer.WaveUtils
{
    public enum WaveFormats
    {
        Pcm = 1,
        Float = 3
    }

    /// <summary>Format header information of a RIFF Wave file.</summary>
	[StructLayout(LayoutKind.Sequential)]
	public class WaveFormat
    {
        #region Members

        private short wFormatTag;
        private short nChannels;
        private int nSamplesPerSec;
        private int nAvgBytesPerSec;
        private short nBlockAlign;
        private short wBitsPerSample;
        private short cbSize;

        #endregion // Members

        #region Properties

        /// <summary>wFormatTag header field.</summary>
        public short FormatTag
        {
            get { return wFormatTag; }
            set { wFormatTag = value; }
        }

        /// <summary>nChannels header field.</summary>
        public short Channels
        {
            get { return nChannels; }
            set { nChannels = value; }
        }

        /// <summary>nSamplesPerSec header field.</summary>
        public int SamplesPerSec
        {
            get { return nSamplesPerSec; }
            set { nSamplesPerSec = value; }
        }

        /// <summary>nAvgBytesPerSec header field.</summary>
        public int AvgBytesPerSec
        {
            get { return nAvgBytesPerSec; }
            set { nAvgBytesPerSec = value; }
        }

        /// <summary>nBlockAlign header field.</summary>
        public short BlockAlign
        {
            get{ return nBlockAlign; }
            set { nBlockAlign = value; }
        }

        /// <summary>wBitsPerSample header field.</summary>
        public short BitsPerSample
        {
            get { return wBitsPerSample; }
            set { wBitsPerSample = value; }
        }

        /// <summary>cbSize header field.</summary>
        public short Size
		{
			get { return cbSize; }
			set { cbSize = value; }
		}

        #endregion // Properties

        #region Constructors

        /// <summary>Constructor.</summary>
        public WaveFormat()
		{
			//default constructor
		}

        /// <summary>Constructor.</summary>
        /// <param name="samplesPerSec">Number of samples per second.</param>
        /// <param name="bitsPerSample">Number of bits per sample.</param>
        /// <param name="channels">Number of channels.</param>
		public WaveFormat(int samplesPerSec, short bitsPerSample, short channels)
		{
			wFormatTag = (short)WaveFormats.Pcm;
			nChannels = channels;
			nSamplesPerSec = samplesPerSec;
			wBitsPerSample = bitsPerSample;
			cbSize = 0;

			nBlockAlign = (short)(channels * (bitsPerSample / 8));
			nAvgBytesPerSec = samplesPerSec * nBlockAlign;
		}

        #endregion // Constructors
    }
}
