using System;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using SoundComparer.DigitalFilter;

namespace SoundComparer.WaveUtils
{
    /// <summary>Stores format and samples of a wave sound.</summary>
	public class WaveSound
    {
        #region Members

        private WaveFormat format;
        private short[] samples;
        short[] a = new short[256];
        private static int fftPoint = 512;
        private int framelen = 512;
        private float[] A; //amplitude (in decibel), each array stores 512 data (1 frame 232,2 ms)
        private string filename="";
        private int nbframe = 0;
        private FIRFilters firFilter;
        private int comparedFrame = 0;
        private string missData="";
        private const int MAX_PROGRESS = 500;

        #endregion // Members

        #region Properties

        /// <summary>Returns the format header information.</summary>
        public WaveFormat Format
        {
            get { return format; }
        }

        /// <summary>Returns the user control for audio graphics.</summary>
        public WaveControl GraphicControl
        {
            get;
            set;
        }

        /// <summary>Returns the progress bar.</summary>
        public ProgressBar ProgressBar
        {
            get;
            set;
        }

        /// <summary>Returns the number of samples.</summary>
        public int Count
        {
            get { return samples.Length; }
        }

        /// <summary>Returns the amplitude.</summary>
        public float[] Amplitude
        {
            get { return A; }
        }

        /// <summary>Returns the sample at the given position.</summary>
        public short this[int indexer]
        {
            get { return samples[indexer]; }
            set { samples[indexer] = value; }
        }

        /// <summary>Returns the wave samples.</summary>
        public short[] Samples
        {
            get { return samples; }
        }

        /// <summary>Returns the number of frame to be analyzed and displayed.</summary>
        public int NbFrame
        {
            get { return nbframe; }
        }

        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        public int FFTPoint
        { 
            set { fftPoint = value; }
        }

        public int Frame
        {
            set { framelen = value; }
        }

        #endregion // Properties

        #region Constructors

        /// <summary>Constructor.</summary>
        /// <param name="format">Format header information.</param>
        /// <param name="samples">Wave samples.</param>
        public WaveSound(WaveFormat format, short[] samples)
        {           
            for (short i = -128; i < 0; i++)
                a[128 + i] = i;
            for (short i = 0; i < 128; i++)
                a[128 + i] = i;

            firFilter = new FIRFilters();
            this.format = format;
            this.samples = samples;
        }

        /// <summary>Constructor.</summary>
        /// <param name="fileName">The path of WAV file.</param>
        /// <param name="graphicControl">The graphic panel where the WAV data will be rendered.</param>
        /// <param name="progressBar">The progress bar to show progress report during loading and reading the WAV file.</param>
        public WaveSound(string fileName, WaveControl graphicControl, ProgressBar progressBar)
        {
            // Initialize UI settings (user control for graphic control and progress bar)
            GraphicControl = graphicControl;
            ProgressBar = progressBar;
            ProgressBar.Maximum = MAX_PROGRESS;
            ProgressBar.Value = 0;
            ProgressBar.Step = 1;
            
            for (short i = -128; i < 0; i++)
                a[128 + i] = i;
            for (short i = 0; i < 128; i++)
                a[128 + i] = i;

            firFilter = new FIRFilters();
            filename = fileName;
            samples = new short[4096];     
        }

        #endregion // Constructors

        #region Functions to process WAV

        /// <summary>Reads a wave file by using background worker since it must report the progress.</summary>
        /// <returns></returns>
        public void ReadWavFile()
        {
            // Create background worker and run it
            using (var bgw = new BackgroundWorker())
            {
                bgw.ProgressChanged += bgw_ProgressChanged;
                bgw.DoWork += bgw_DoWork;
                bgw.RunWorkerCompleted += bgw_WorkCompleted;
                bgw.WorkerReportsProgress = true;
                bgw.RunWorkerAsync();
            }
        }

        /// <summary>Reads a wave file and keep reporting the progress.</summary>
        /// <param name="sender">The control that the action is for</param> 
        /// <param name="e">The arguments of the event</param>
        /// <returns></returns>
        public void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            // Read WAV file and write FFT file
            using (BinaryReader reader = new BinaryReader(new FileStream(filename, FileMode.Open)))
            using (BinaryWriter fft = new BinaryWriter(File.Open(filename + ".fft.dat", FileMode.Create)))
            {
                this.format = ReadHeader(reader);
                if (format.BitsPerSample != 8 && format.BitsPerSample != 16)
                {
                    System.Windows.Forms.MessageBox.Show("The sound has " + format.BitsPerSample + " bits per sample. Please choose a sound with 8 or 16 bits per sample.");
                    return;
                }

                int bytesPerSample = format.BitsPerSample / 8;
                int dataLength = reader.ReadInt32();
                int countSamples = dataLength / bytesPerSample;

                // Throw interlude before recording
                int di = 5; // every second, take 2205 data or every 5 data take 1 data
                int b = reader.ReadByte();
                int adv = 1;
                while (b > 120 && b < 140)
                {
                    reader.ReadBytes(di - 1);
                    b = reader.ReadByte();
                    adv += di;
                }
                countSamples = countSamples - adv;

                float[] channelSamples16 = new float[framelen]; //buffer data per 1 frame        

                int si = countSamples / (samples.Length * di);
                if (countSamples % (si * (samples.Length * di)) != 0)
                {
                    si++;
                }

                int overlap = (int)(0 * framelen);

                float[] sampelOverlap = null;
                if (overlap > 0)
                    sampelOverlap = new float[overlap];
                int nbsi = (framelen - overlap) / si;
                while (nbsi == 0)
                {
                    si = si - framelen;
                    nbsi = (framelen - overlap) / si;
                }
                if ((framelen - overlap) % (nbsi * si) != 0)
                {
                    nbsi++;
                }
                countSamples = countSamples - (overlap * di);
                nbframe = countSamples / ((framelen - overlap) * di);
                // (countSamples-overlap) because at the begining we already read the data before entering region 'Read Data'
                if (countSamples % ((framelen - overlap) * di) != 0)
                {
                    nbframe += 1;
                }
                A = new float[fftPoint / 2]; //buffer hasil fft 1 frame (output applyFFT)
                int channelSamplesIndex = 0;
                int endloop = 0;
                byte channelSample8;
                Int16 sample16;
                int endfft = 0;
                firFilter.FreqFrom = 500;
                firFilter.FreqTo = 4000;
                firFilter.CalculateCoefficients(FFT.Algorithm.Blackman, FFT.FilterType.BandPass);

                // Read overlapped sample
                channelSamplesIndex = 0;
                if (format.BitsPerSample == 8)
                {
                    for (int sampleIndex = 0; sampleIndex < overlap; sampleIndex++)
                    {
                        channelSample8 = reader.ReadByte();
                        sampelOverlap[channelSamplesIndex] = (short)((a[(short)(channelSample8)]) << 8);
                        channelSamplesIndex++;
                        reader.ReadBytes(di - 1);
                        sampleIndex += di - 1;
                    }
                }
                else
                {
                    for (int sampleIndex = 0; sampleIndex < overlap; sampleIndex++)
                    {
                        sample16 = reader.ReadInt16();
                        sampelOverlap[channelSamplesIndex] = reader.ReadInt16();
                        channelSamplesIndex++;
                        reader.ReadBytes(2 * (di - 1));
                        sampleIndex += di - 1;
                    }
                }

                // Set the progress value
                int progressInPercentage = 0;
                int factor = 100;
                float progressValue = 0;
                float dprogressvalue = (float)factor / (float)nbframe;
                while ((int)dprogressvalue >= 100 || (int)dprogressvalue == 0)
                {
                    factor *= 10;
                    dprogressvalue = factor / nbframe;
                }

                for (int i = 0; i < nbframe; i++)
                {
                    // Get overlapped component
                    for (int sampleIndex = 0; sampleIndex < overlap; sampleIndex++)
                    {
                        channelSamples16[sampleIndex] = sampelOverlap[sampleIndex];
                    }
                    endloop = di * (channelSamples16.Length - overlap);
                    if (i == (nbframe - 1)) // if it's the last loop, the buffer may not be used as a whole
                    {
                        endloop = countSamples % endloop;
                    }
                    channelSamplesIndex = overlap;
                    
                    #region Read Data

                    if (format.BitsPerSample == 8)
                    {
                        try
                        {
                            for (int sampleIndex = overlap; sampleIndex < (endloop + overlap) && channelSamplesIndex < framelen; sampleIndex++)
                            {
                                channelSample8 = reader.ReadByte();
                                channelSamples16[channelSamplesIndex] = (short)((a[(short)(channelSample8)]) << 8);
                                channelSamplesIndex++;
                                reader.ReadBytes(di - 1);
                                sampleIndex += di - 1;
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            for (int sampleIndex = 0; sampleIndex < countSamples && channelSamplesIndex < (framelen); sampleIndex++)
                            {
                                sample16 = reader.ReadInt16();
                                channelSamples16[channelSamplesIndex] = reader.ReadInt16();
                                channelSamplesIndex++;
                                reader.ReadBytes(2 * (di - 1));
                                sampleIndex += di - 1;
                            }
                        }
                        catch { }
                    }

                    #endregion // Read Data

                    // Get data to be displayed
                    for (int j = 0; j < nbsi && (j + i * nbsi) < samples.Length; j++)
                    {
                        samples[j + i * nbsi] = (short)channelSamples16[j * si];
                    }

                    if (i != (nbframe - 1))
                    {
                        // Update overlapped component
                        for (int sampleIndex = framelen - overlap; sampleIndex < framelen; sampleIndex++)
                        {
                            sampelOverlap[sampleIndex - (framelen - overlap)] = channelSamples16[sampleIndex];
                        }
                    }

                    // Band pass the signal filter
                    FilterCalculation(ref channelSamples16, FFT.FilterType.BandPass);

                    ApplyFFT(channelSamples16);

                    endfft = fftPoint / 2;

                    if (channelSamplesIndex != 0)
                    {
                        if (endfft > channelSamplesIndex / 2)
                            endfft = channelSamplesIndex / 2;
                    }
                    for (int j = 1; j < endfft; j++)
                    {
                        fft.Write((Int16)A[j]);
                    }
                    fft.Write(-9999);

                    // Report the progress to Progress Bar
                    progressValue += dprogressvalue;
                    if (progressInPercentage != (int)(((float)progressValue / (float)factor) * (MAX_PROGRESS * 0.9))) // put 90% for reading, the 10% for redrawing the control
                    {
                        progressInPercentage = (int)(((float)progressValue / (float)factor) * (MAX_PROGRESS * 0.9));
                        ((BackgroundWorker)sender).ReportProgress(progressInPercentage);
                    }
                }
                
                reader.Close();
                fft.Close();
            }
        }

        /// <summary>Finishing the progress bar and refreshing the graphic control after reading WAV file is completed.</summary>
        /// <param name="sender">The control that the action is for</param> 
        /// <param name="e">The arguments of the event</param>
        /// <returns></returns>
        void bgw_WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressBar.Value = MAX_PROGRESS;
            GraphicControl.Refresh();
        }

        /// <summary>Refreshing the progress bar when the progress event is triggered.</summary>
        /// <param name="sender">The control that the action is for</param> 
        /// <param name="e">The arguments of the event</param>
        /// <returns></returns>
        void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar.PerformStep();
        }

        /// <summary>Read a chunk of four bytes from a wave file</summary>
        /// <param name="reader">Reader for the wave file.</param>
        /// <returns>Four characters.</returns>
        private string ReadChunk(BinaryReader reader)
        {
            byte[] ch = new byte[4];
            reader.Read(ch, 0, ch.Length);
            return System.Text.Encoding.ASCII.GetString(ch,0,4);
        }

        /// <summary>
        /// Read the header from a wave file, and move the
        /// reader's position to the beginning of the data chunk
        /// </summary>
        /// <param name="reader">Reader for the wave file.</param>
        /// <returns>Format of the wave.</returns>
        private WaveFormat ReadHeader(BinaryReader reader)
        {
            if (ReadChunk(reader) != "RIFF")
                throw new Exception("Invalid file format");

            reader.ReadInt32(); // File length minus first 8 bytes of RIFF description, we don't use it

            if (ReadChunk(reader) != "WAVE")
                throw new Exception("Invalid file format");

            if (ReadChunk(reader) != "fmt ")
                throw new Exception("Invalid file format");

            int len = reader.ReadInt32();
            if (len < 16) // bad format chunk length
                throw new Exception("Invalid file format");

            WaveFormat carrierFormat = new WaveFormat();
            carrierFormat.FormatTag = reader.ReadInt16();
            carrierFormat.Channels = reader.ReadInt16();
            carrierFormat.SamplesPerSec = reader.ReadInt32();
            carrierFormat.AvgBytesPerSec = reader.ReadInt32();
            carrierFormat.BlockAlign = reader.ReadInt16();
            carrierFormat.BitsPerSample = reader.ReadInt16();

            // Advance in the stream to skip the wave format block 
            len -= 16; // minimum format size
            while (len > 0)
            {
                reader.ReadByte();
                len--;
            }

            // Assume the data chunk is aligned
            string chunk;
            do
            {
                chunk = ReadChunk(reader);
            } while (reader.BaseStream.Position < reader.BaseStream.Length && chunk != "data");

            return carrierFormat;
        }

        #endregion // Functions to process WAV

        #region Functions for FFT

        /// <summary>Compare its wave sound to the given sound by applying FFT algorithm.</summary>
        /// <param name="wf">The wave sound that is used to compare with its own wave sound</param> 
        /// <returns>percentage of similarity</returns>
        public float Compare(WaveSound wf)
        {
            int nbcorrect=0;
            int i=0;

            bool endfile = false;
            int a = 0, b = 0;
            float[] frames = new float[nbframe > wf.nbframe ? nbframe : wf.nbframe];
            comparedFrame = nbframe < wf.nbframe ? nbframe : wf.nbframe;
            short[] MissPlay = new short[comparedFrame];
            int step = 0;
            if (ProgressBar.Value != MAX_PROGRESS) step = (MAX_PROGRESS - ProgressBar.Value) / comparedFrame;

            // Open FFT file
            using (BinaryReader f1 = new BinaryReader(File.Open(filename + ".fft.dat", FileMode.Open)))
            using (BinaryReader f2 = new BinaryReader(File.Open(wf.filename + ".fft.dat", FileMode.Open)))
            using (StreamWriter sw2 = new StreamWriter(wf.filename + ".fft.txt"))
            {
                while (!endfile)
                {
                    try
                    {
                        a = f1.ReadInt16();
                        b = f2.ReadInt16();
                        if (i == 49)
                        { }
                        if (a == -9999 || b == -9999)
                        {
                            // Calculate the value for 1 frame
                            frames[i] = (float)nbcorrect / (fftPoint / 2);
                            ProgressBar.Value += step;
                            i++;
                            nbcorrect = 0;
                        }
                        else
                        {
                            if (Math.Abs(a - b) <= 10)
                            {
                                nbcorrect++;
                            }
                        }
                        sw2.Write(a);
                        sw2.WriteLine("," + b);
                    }
                    catch
                    {
                        endfile = true;
                        f1.Close();
                        f2.Close();
                        sw2.Close();
                    }
                }

                nbcorrect = 0;
                int idx = 0;
                int len = comparedFrame;
                comparedFrame = 0;

                for (i = 0; i < len; i++)
                {
                    nbcorrect += frames[i] >= 0.7 ? 1 : 0;
                    if (frames[i] < 0.7)
                    {
                        MissPlay[idx] = (short)i;
                        idx = idx + 1;
                        comparedFrame++;
                    }
                }

                f1.Close();
                f2.Close();
                sw2.Close();
                for (i = 0; i < comparedFrame; i++)
                    missData = missData + MissPlay[i] + ";";
            }

            return (float)(nbcorrect * 100 / frames.Length);
        }

        /// <summary>Applying FFT</summary>
        /// <param name="data">The data that needs to be processed through FFT processor</param> 
        public void ApplyFFT(float[] data)
        {
            float[] a2 = new float[fftPoint]; //temp real
            float[] a1 = new float[fftPoint]; //temp imaginary
            int loop = data.Length / fftPoint;
            if ((loop * fftPoint) < data.Length)
                loop += 1;
            int inloop, k = 0, stop,t;
            int log2n = ilog2(fftPoint);
            float maxA = float.MinValue,minA=float.MaxValue;
            float refA=0;
            for (int i = 0; i < loop; i++)
            {
                k = 0; 
                inloop = i * fftPoint;
                stop = inloop + fftPoint;
                if (stop > data.Length)
                    stop = data.Length;
                
                // Reorder input and split input into real and complex parts 

                for (int j = inloop; j < stop; j++)
                {
                    t = bitrev(k, log2n);
                    a1[t] = data[j];
                    a2[t] = 0.0f;
                    k++;
                }

                if (stop % fftPoint != 0)
                {
                    for (int l = stop % fftPoint; l < fftPoint; l++)
                    {
                        t = bitrev(l, log2n);
                        a1[t] = 0f;
                        a2[t] = 0.0f;
                    }
                }

                fft(log2n,ref a2, ref a1, fftPoint, 1);
                
                k = 0;
                a1[0] = 0; a2[0] = 0;
                maxA = float.MinValue;
                minA = float.MaxValue;
                for (int v = inloop/2; v < stop/2; v++) {
                    A[v] = (a1[k] * a1[k]) + (a2[k] * a2[k]);
                    maxA = Math.Max(A[v], maxA);
                    minA = Math.Min(A[v], minA);
                    k++;
                }
                refA = maxA - minA;

                for (int v = inloop / 2 + 1; v < stop / 2; v++)
                    A[v] = 10f * (float)Math.Log10(A[v]/refA);
                for (int v = stop / 2; v < fftPoint/2; v++)
                    A[v] = 0;
            }
        }

        /// <summary>Perform FFT</summary>
        void fft(int log2n, ref float[] a2, ref float[] a1, int n, int sgn)
        {
            int i, j, k, k2, s, m;
            float wm1, wm2, w1, w2, t1, t2, u1, u2;

            // Loop on FFT stages
            for (s = 1; s <= log2n; s++)
            {

                m = 1 << s;			/* m = 2^s */
                wm1 = (float)Math.Cos(sgn * 2 * Math.PI / m);	/* wm = exp(q*2*pi*i/m); */
                wm2 = (float)Math.Sin(sgn * 2 * Math.PI / m);

                w1 = 1.0f;
                w2 = 0.0f;

                for (j = 0; j < m / 2; j++)
                {
                    for (k = j; k < n; k += m)
                    {
                        /* t = w*a[k+m/2]; */
                        k2 = k + m / 2;
                        t1 = w1 * a1[k2] - w2 * a2[k2];
                        t2 = w1 * a2[k2] + w2 * a1[k2];

                        u1 = a1[k];
                        u2 = a2[k];

                        a1[k] = u1 + t1;
                        a2[k] = u2 + t2;

                        a1[k2] = u1 - t1;
                        a2[k2] = u2 - t2;
                    }
                    /* w = w * wm; */
                    t1 = w1 * wm1 - w2 * wm2;
                    w2 = w1 * wm2 + w2 * wm1;
                    w1 = t1;
                }
            }

            // Flip the final stage
            for (i = 1; i < n / 2; i++)
            {
                t1 = a1[i];
                a1[i] = a1[n - i];
                a1[n - i] = t1;
                t2 = a2[i];
                a2[i] = a2[n - i];
                a2[n - i] = t2;
            }
            if (sgn == -1)
            {
                for (i = 0; i < n; i++)
                {
                    a1[i] /= (float)n;
                    a2[i] /= (float)n;
                }
            }
        }

        /// <summary>Get logarithm of 2</summary>
        private int ilog2(int n)
        {
            int i;
            for (i = 8 * sizeof(int) - 1; i >= 0 && ((1 << i) & n) == 0; i--) ;
            return i;
        }

        /// <summary>Reverse bits 0 thru k-1 in the integer "a"</summary>
        int bitrev(int a, int k)
        {
            int i, b, p, q;
            for (i = b = 0, p = 1, q = 1 << (k - 1);
                 i < k;
                 i++, p <<= 1, q >>= 1) if ((a & q) > 0) b |= p;
            return b;
        }

        #endregion // Functions for FFT

        #region Functions for Digital Filter

        /// <summary>
        /// Performs the actual filtering operation with respect to input from the combo boxes.
        /// </summary>
        private void FilterCalculation(ref float[] input, FFT.FilterType filter)
        {            
            // Check what kind of filter the user wanted and filter accordingly
            switch (filter)
            {
                case FFT.FilterType.HighPass:
                    firFilter.HighPassFilter(ref input);
                    break;

                case FFT.FilterType.LowPass:
                    firFilter.LowPassFilter(ref input);
                    break;

                case FFT.FilterType.BandPass:
                    firFilter.BandPassFilter(ref input);
                    break;
            }
        }

        #endregion // Functions for Digital Filter
    }
}
