﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RelhaxModpack
{
    class ConfigWPFComboBox : System.Windows.Controls.ComboBox
    {
        public Catagory catagory { get; set; }
        public Mod mod { get; set; }
        public Config config { get; set; }
        public SubConfig subconfig { get; set; }
    }
}
