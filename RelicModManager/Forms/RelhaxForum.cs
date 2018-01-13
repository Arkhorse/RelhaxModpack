﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace RelhaxModpack
{
    public class RelhaxForum : Form
    {
        protected override void OnLoad(EventArgs e)
        {
            SuspendLayout();
            base.OnLoad(e);
            if (Settings.AppScalingMode == AutoScaleMode.Dpi)
            {
                AutoScaleDimensions = new SizeF(96F, 96F);//for design in 96 DPI
                AutoScaleMode = Settings.AppScalingMode;
                Scale(new SizeF(Settings.ScaleSize, Settings.ScaleSize));
            }
            else
            {
                AutoScaleMode = Settings.AppScalingMode;
            }
            Font = Settings.AppFont;
            //set the UI colors
            Settings.setUIColor(this);
            ResumeLayout(false);
            OnPostLoad();
        }
        public virtual void OnPostLoad()
        {
            //stub, to be overridden
            //so that any code that should run after UI scaling can be done
        }
    }
}
