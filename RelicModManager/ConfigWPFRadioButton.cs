﻿namespace RelhaxModpack
{
    class ConfigWPFRadioButton : System.Windows.Controls.RadioButton, UIComponent
    {
        public Category catagory { get; set; }
        public Mod mod { get; set; }
        public Config config { get; set; }
    }
}
