﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RelicModManager
{
    public class Catagory
    {
        public string name { get; set; }
        public string selectionType { get; set; }
        public List<Mod> mods = new List<Mod>();
        public Catagory() { }
    }
}
