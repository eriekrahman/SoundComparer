using System;
using System.IO;
using System.Media;
using System.Windows.Forms;
using SoundComparer.WaveUtils;

namespace SoundComparer
{
    public partial class FormComparer : Form
    {
        private WaveControl wc { set; get; }
        private WaveControl wc2 { set; get; }
        private string LastPath { set; get; }

        public FormComparer()
        {
            InitializeComponent();
            pSound1.Controls.Clear();
            pSound2.Controls.Clear();
        }

        private void btnBrowseSound1_Click(object sender, EventArgs e)
        {
            ofdSound = new OpenFileDialog();
            ofdSound.InitialDirectory = String.IsNullOrEmpty(LastPath) ? "c:\\" : LastPath;
            ofdSound.Filter = "txt files (*.wav)|*.wav|All files (*.*)|*.*";
            ofdSound.FilterIndex = 2;
            ofdSound.RestoreDirectory = true;            

            if (ofdSound.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Process wave control
                    wc = new WaveControl();
                    wc.Name = ofdSound.FileName;
                    LastPath = Path.GetDirectoryName(ofdSound.FileName);
                    wc.Read(wc, ofdSound.FileName, pbSound1);
                    wc.Refresh();

                    // Add wave result
                    pSound1.Controls.Clear();
                    pSound1.Controls.Add(wc);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while reading the sound file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnPlaySound1_Click(object sender, EventArgs e)
        {
            try
            {
                if (wc != null && File.Exists(wc.Name))
                {
                    SoundPlayer simpleSound = new SoundPlayer(wc.Name);
                    simpleSound.Play();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while playing the sound file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBrowseSound2_Click(object sender, EventArgs e)
        {
            ofdSound = new OpenFileDialog();
            ofdSound.InitialDirectory = String.IsNullOrEmpty(LastPath)? "c:\\": LastPath;
            ofdSound.Filter = "txt files (*.wav)|*.wav|All files (*.*)|*.*";
            ofdSound.FilterIndex = 2;
            ofdSound.RestoreDirectory = true;

            if (ofdSound.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Process wave control
                    wc2 = new WaveControl();
                    wc2.Name = ofdSound.FileName;
                    LastPath = Path.GetDirectoryName(ofdSound.FileName);
                    wc2.Read(wc2, ofdSound.FileName, pbSound2);
                    wc2.Refresh();

                    // Add wave result
                    pSound2.Controls.Clear();
                    pSound2.Controls.Add(wc2);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while reading the sound file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnPlaySound2_Click(object sender, EventArgs e)
        {
            try
            {
                if (wc2 != null && File.Exists(wc2.Name))
                {
                    SoundPlayer simpleSound = new SoundPlayer(wc2.Name);
                    simpleSound.Play();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while playing the sound file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            if (wc == null)
                MessageBox.Show("The first sound should be loaded first before analyzing", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning );
            else if (wc2 == null)
                MessageBox.Show("The second sound should be loaded first before analyzing", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                wc.DrawWave = true;
                wc2.DrawWave = true;

                try
                {
                    // Compare 2 ware controls
                    float accuracy = wc2.Sound.Compare(wc.Sound);
                    lbAnalyze.Text = "Accuracy = " + accuracy + "%";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while analyzing the sound files: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
