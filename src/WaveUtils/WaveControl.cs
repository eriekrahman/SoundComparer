using System;
using System.Drawing;
using System.Windows.Forms;

namespace SoundComparer.WaveUtils
{
	/// <summary>
	/// Summary description for WaveControl.
	/// </summary>
	public class WaveControl : System.Windows.Forms.UserControl
    {
        #region Members

        /// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// This is the WaveFile class variable that describes the internal structures of the .WAV
		/// </summary>
		//private WaveFile		m_Wavefile;
        private WaveSound m_Wavefile;

		/// <summary>
		/// Boolean for whether the .WAV should draw or not.  So that the control doesnot draw the .WAV until after it is read
		/// </summary>
		private bool m_DrawWave = false;

		/// <summary>
		/// Filename string
		/// </summary>
		//private string			m_Filename;

		/// <summary>
		/// Each pixel value (X direction) represents this many samples in the wavefile
		/// Starting value is based on control width so that the .WAV will cover the entire width.
		/// </summary>
		private float m_SamplesPerPixel = 0f;

		/// <summary>
		/// This value is the amount to increase/decrease the m_SamplesPerPixel.  This creates a 'Zoom' affect.
		/// Starting value is m_SamplesPerPixel / 25    so that it is scaled for the size of the .WAV
		/// </summary>
		private float m_ZoomFactor;

		/// <summary>
		/// This is the starting x value of a mouse drag
		/// </summary>
		private int	m_StartX = 0;

		/// <summary>
		/// This is the ending x value of a mouse drag
		/// </summary>
		private int	m_EndX = 0;
        
		/// <summary>
		/// Offset from the beginning of the wave for where to start drawing
		/// </summary>
		private int	m_OffsetInSamples = 0;

        #endregion // Members

        #region Properties

        public bool DrawWave
        {
            set { m_DrawWave = value; }
        }

        public WaveSound Sound
        {
            get { return m_Wavefile; }
        }

		private float SamplesPerPixel
		{
			set
			{
				m_SamplesPerPixel = value;
				m_ZoomFactor = m_SamplesPerPixel / 25;
			}
		}

        #endregion // Properties

        #region Constructor

        public WaveControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

        #endregion // Constructor

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // WaveControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.Lavender;
            this.ForeColor = System.Drawing.Color.Lime;
            this.Name = "WaveControl";
            this.Size = new System.Drawing.Size(530, 101);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.WaveControl_Paint);
            this.ResumeLayout(false);
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion // Component Designer generated code

        #region Wave Drawing

        public void Read(WaveControl wc, string filename, ProgressBar progress )
		{
            m_Wavefile = new WaveSound(filename, wc, progress);
            m_Wavefile.ReadWavFile();
            m_DrawWave = true;
		}

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Left empty, avoids undesirable flickering
        }

		private void Draw( PaintEventArgs pea, Pen pen )
		{
			Graphics grfx = pea.Graphics;
            Rectangle visBounds = ClientRectangle;

			if ( m_SamplesPerPixel == 0.0 )
			{
                this.SamplesPerPixel = (m_Wavefile.Samples.Length / visBounds.Width);
			}

            grfx.DrawLine(pen, 0, (int)visBounds.Height / 2, (int)visBounds.Width, (int)visBounds.Height / 2);
            Draw16Bit( grfx, pen, visBounds );
		}

		private void Draw16Bit( Graphics grfx, Pen pen, RectangleF visBounds )
		{
			int prevX = 0;
			int prevY = 0;

			int i = 0;			
			int index = m_OffsetInSamples; // index is how far to offset into the data array
            int maxSampleToShow = (int) (( m_SamplesPerPixel * visBounds.Width ) + m_OffsetInSamples);
            
            maxSampleToShow = Math.Min(maxSampleToShow, m_Wavefile.Samples.Length);
			while ( index < maxSampleToShow )
			{
				short maxVal = -32767;
				short minVal = 32767;

				// Finds the max & min peaks for this pixel 
				for ( int x = 0; x < m_SamplesPerPixel; x++ )
				{
                    maxVal = Math.Max( maxVal, m_Wavefile.Samples[ x + index ] );
					minVal = Math.Min( minVal, m_Wavefile.Samples[ x + index ] );
				}

				// Scales based on height of window
				int scaledMinVal = (int) (( (minVal + 32768) * visBounds.Height ) / 65536 );
				int scaledMaxVal = (int) (( (maxVal + 32768) * visBounds.Height ) / 65536 );

				//  If samples per pixel is small or less than zero, we are out of zoom range, so don't display anything
				if ( m_SamplesPerPixel > 0.0000000001 )
				{
					// If the max/min are the same, then draw a line from the previous position,
					// otherwise we will not see anything
					if ( scaledMinVal == scaledMaxVal )
					{
						if ( prevY != 0 )
							grfx.DrawLine( pen, prevX, prevY, i, scaledMaxVal );
					}
					else
					{
						grfx.DrawLine( pen, i, scaledMinVal, i, scaledMaxVal );
					}
				}
				else
					return;

				prevX = i;
				prevY = scaledMaxVal;
				
				i++;
				index = (int) ( i * m_SamplesPerPixel) + m_OffsetInSamples;
			}
        }

        private void WaveControl_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(Color.Black);
            SolidBrush brush = new SolidBrush(this.BackColor);
            e.Graphics.FillRectangle(brush, 0, 0, (int)e.Graphics.ClipBounds.Width, (int)e.Graphics.ClipBounds.Height);
            if (m_DrawWave)
            {
                Draw(e, pen);
            }

            int regionStartX = Math.Min(m_StartX, m_EndX);
            int regionEndX = Math.Max(m_StartX, m_EndX);

            brush = new SolidBrush(Color.Violet);
            e.Graphics.FillRectangle(brush, regionStartX, 0, regionEndX - regionStartX, (int)e.Graphics.ClipBounds.Height);
        }

        #endregion // Wave Drawing
    }
}
