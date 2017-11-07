﻿using System;
using System.Windows.Forms;

namespace RelhaxModpack
{
    public partial class CRCCheck : Form
    {
        public CRCCheck()
        {
            InitializeComponent();
        }
        //handler to get the CRC of the file
        private void button1_Click(object sender, EventArgs e)
        {
            //unable to find it in the registry, so ask for it
            if (openFileDialog1.ShowDialog().Equals(DialogResult.Cancel))
            {
                return;
            }
            string crc = XMLUtils.GetMd5Hash(openFileDialog1.FileName);
            crcTB.Text = crc;
        }

        private void CRCCheck_Load(object sender, EventArgs e)
        {
            //font scaling
            this.AutoScaleMode = Settings.AppScalingMode;
            this.Font = Settings.AppFont;
            if (Settings.AppScalingMode == System.Windows.Forms.AutoScaleMode.Dpi)
            {
                this.Scale(new System.Drawing.SizeF(Settings.ScaleSize, Settings.ScaleSize));
            }
        }

        private void CRCCheck_FormClosing(object sender, FormClosingEventArgs e)
        {
            Utils.AppendToLog("|------------------------------------------------------------------------------------------------|");
        }
    }
}
