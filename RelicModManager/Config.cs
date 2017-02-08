﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RelicModManager
{
    public class Config
    {
        public string name { get; set; }
        public string zipConfigFile { get; set; }
        public string crc { get; set; }
        public bool enabled { get; set; }
        public bool configChecked { get; set; }
        //public string patchFileName { get; set; }
        public string type { get; set; }
        public List<string> pictureList = new List<string>();

        public Config()
        {
            //by default make these false
            enabled = false;
            configChecked = false;
        }
    }
}
