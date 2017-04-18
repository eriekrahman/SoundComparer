using System.Drawing;

namespace SoundComparer
{
    partial class FormComparer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormComparer));
            this.btnAnalyze = new System.Windows.Forms.Button();
            this.pbSound1 = new System.Windows.Forms.ProgressBar();
            this.pbSound2 = new System.Windows.Forms.ProgressBar();
            this.lbAnalyze = new System.Windows.Forms.TextBox();
            this.pSound2 = new System.Windows.Forms.Panel();
            this.ofdSound = new System.Windows.Forms.OpenFileDialog();
            this.btnBrowseSound1 = new System.Windows.Forms.Button();
            this.btnBrowseSound2 = new System.Windows.Forms.Button();
            this.pSound1 = new System.Windows.Forms.Panel();
            this.btnPlaySound1 = new System.Windows.Forms.Button();
            this.btnPlaySound2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnAnalyze
            // 
            this.btnAnalyze.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAnalyze.Location = new System.Drawing.Point(12, 286);
            this.btnAnalyze.Name = "btnAnalyze";
            this.btnAnalyze.Size = new System.Drawing.Size(801, 72);
            this.btnAnalyze.TabIndex = 0;
            this.btnAnalyze.Text = "Analyze";
            this.btnAnalyze.UseVisualStyleBackColor = true;
            this.btnAnalyze.Click += new System.EventHandler(this.btnAnalyze_Click);
            // 
            // pbSound1
            // 
            this.pbSound1.Location = new System.Drawing.Point(12, 119);
            this.pbSound1.Maximum = 500;
            this.pbSound1.Name = "pbSound1";
            this.pbSound1.Size = new System.Drawing.Size(530, 23);
            this.pbSound1.TabIndex = 2;
            // 
            // pbSound2
            // 
            this.pbSound2.Location = new System.Drawing.Point(12, 255);
            this.pbSound2.Maximum = 500;
            this.pbSound2.Name = "pbSound2";
            this.pbSound2.Size = new System.Drawing.Size(530, 23);
            this.pbSound2.TabIndex = 3;
            // 
            // lbAnalyze
            // 
            this.lbAnalyze.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbAnalyze.Enabled = false;
            this.lbAnalyze.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAnalyze.Location = new System.Drawing.Point(12, 367);
            this.lbAnalyze.Name = "lbAnalyze";
            this.lbAnalyze.Size = new System.Drawing.Size(801, 31);
            this.lbAnalyze.TabIndex = 4;
            this.lbAnalyze.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // pSound2
            // 
            this.pSound2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pSound2.Location = new System.Drawing.Point(12, 150);
            this.pSound2.Name = "pSound2";
            this.pSound2.Size = new System.Drawing.Size(530, 99);
            this.pSound2.TabIndex = 5;
            // 
            // ofdSound
            // 
            this.ofdSound.FileName = "openFileDialog1";
            // 
            // btnBrowseSound1
            // 
            this.btnBrowseSound1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnBrowseSound1.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowseSound1.Image")));
            this.btnBrowseSound1.Location = new System.Drawing.Point(548, 12);
            this.btnBrowseSound1.Name = "btnBrowseSound1";
            this.btnBrowseSound1.Size = new System.Drawing.Size(130, 130);
            this.btnBrowseSound1.TabIndex = 7;
            this.btnBrowseSound1.UseVisualStyleBackColor = true;
            this.btnBrowseSound1.Click += new System.EventHandler(this.btnBrowseSound1_Click);
            // 
            // btnBrowseSound2
            // 
            this.btnBrowseSound2.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowseSound2.Image")));
            this.btnBrowseSound2.Location = new System.Drawing.Point(548, 149);
            this.btnBrowseSound2.Name = "btnBrowseSound2";
            this.btnBrowseSound2.Size = new System.Drawing.Size(130, 130);
            this.btnBrowseSound2.TabIndex = 8;
            this.btnBrowseSound2.UseVisualStyleBackColor = true;
            this.btnBrowseSound2.Click += new System.EventHandler(this.btnBrowseSound2_Click);
            // 
            // pSound1
            // 
            this.pSound1.BackColor = System.Drawing.SystemColors.Control;
            this.pSound1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pSound1.Location = new System.Drawing.Point(12, 12);
            this.pSound1.Name = "pSound1";
            this.pSound1.Size = new System.Drawing.Size(530, 101);
            this.pSound1.TabIndex = 6;
            // 
            // btnPlaySound1
            // 
            this.btnPlaySound1.Image = ((System.Drawing.Image)(resources.GetObject("btnPlaySound1.Image")));
            this.btnPlaySound1.Location = new System.Drawing.Point(683, 12);
            this.btnPlaySound1.Name = "btnPlaySound1";
            this.btnPlaySound1.Size = new System.Drawing.Size(130, 130);
            this.btnPlaySound1.TabIndex = 9;
            this.btnPlaySound1.UseVisualStyleBackColor = true;
            this.btnPlaySound1.Click += new System.EventHandler(this.btnPlaySound1_Click);
            // 
            // btnPlaySound2
            // 
            this.btnPlaySound2.Image = ((System.Drawing.Image)(resources.GetObject("btnPlaySound2.Image")));
            this.btnPlaySound2.Location = new System.Drawing.Point(684, 149);
            this.btnPlaySound2.Name = "btnPlaySound2";
            this.btnPlaySound2.Size = new System.Drawing.Size(130, 130);
            this.btnPlaySound2.TabIndex = 10;
            this.btnPlaySound2.UseVisualStyleBackColor = true;
            this.btnPlaySound2.Click += new System.EventHandler(this.btnPlaySound2_Click);
            // 
            // FormComparer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(825, 414);
            this.Controls.Add(this.btnPlaySound2);
            this.Controls.Add(this.btnPlaySound1);
            this.Controls.Add(this.btnBrowseSound2);
            this.Controls.Add(this.btnBrowseSound1);
            this.Controls.Add(this.pSound2);
            this.Controls.Add(this.lbAnalyze);
            this.Controls.Add(this.pbSound2);
            this.Controls.Add(this.pbSound1);
            this.Controls.Add(this.btnAnalyze);
            this.Controls.Add(this.pSound1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormComparer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sound Comparer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        
        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.ProgressBar pbSound1;
        private System.Windows.Forms.ProgressBar pbSound2;
        private System.Windows.Forms.TextBox lbAnalyze;
        private System.Windows.Forms.Panel pSound2;
        private System.Windows.Forms.Panel pSound1;
        private System.Windows.Forms.OpenFileDialog ofdSound;
        private System.Windows.Forms.Button btnBrowseSound1;
        private System.Windows.Forms.Button btnBrowseSound2;
        private System.Windows.Forms.Button btnPlaySound1;
        private System.Windows.Forms.Button btnPlaySound2;
    }
}

