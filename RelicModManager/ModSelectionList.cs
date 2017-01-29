﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace RelicModManager
{
    public partial class ModSelectionList : Form
    {

        private List<Mod> parsedModsList;
        public List<Catagory> parsedCatagoryList;

        public bool cancel = false;

        public ModSelectionList()
        {
            InitializeComponent();
        }

        private void ModSelectionList_Load(object sender, EventArgs e)
        {
            this.createModStructure2();
            this.makeTabs();
            this.addAllMods();
        }

        private void addAllMods()
        {
            foreach (TabPage t in this.modTabGroups.TabPages)
            {
                foreach (Catagory c in parsedCatagoryList)
                {
                    if (c.name.Equals(t.Text))
                    {
                        //matched the catagory to tab
                        int i = 1;
                        foreach (Mod m in c.mods)
                        {
                            this.addMod(m, t, i++);
                        }
                        break;
                    }
                }
            }
        }

        private void makeTabs()
        {
            modTabGroups.TabPages.Clear();
            foreach (Catagory c in parsedCatagoryList)
            {
                modTabGroups.TabPages.Add(new TabPage(c.name));
            }
        }

        private void addMod(Mod m, TabPage t, int panelCount)
        {

            //make config panel
            Panel configPanel = new Panel();
            configPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            configPanel.Location = new System.Drawing.Point(3, 26);
            //configPanel.Name = "configPanel";
            configPanel.Size = new System.Drawing.Size(this.Size.Width - 60, 30);
            configPanel.TabIndex = 2;
            configPanel.AutoSize = true;
            configPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;
            configPanel.Controls.Clear();
            for (int i = 0; i < m.configs.Count; i++)
            {
                //make configLabel
                Label configLabel = new Label();
                configLabel.AutoSize = true;
                int yPosition = 15 * (i + 1);
                configLabel.Location = new System.Drawing.Point(5, yPosition-10);
                //configLabel.Name = "configLabel";
                configLabel.Size = new System.Drawing.Size(100, 15);
                configLabel.TabIndex = 0;
                configLabel.Text = m.configs[i].name;
                configLabel.Enabled = false;
                configPanel.Controls.Add(configLabel);

                //make configControl
                switch (m.configType)
                {
                    case "single":
                        RadioButton configControlRB = new RadioButton();
                        configControlRB.AutoSize = true;
                        configControlRB.Location = new System.Drawing.Point(100, yPosition-10);
                        //configControlRB.Name = "configControlRB";
                        configControlRB.Size = new System.Drawing.Size(150, 15);
                        configControlRB.TabIndex = 1;
                        configControlRB.TabStop = true;
                        configControlRB.Enabled = false;
                        configControlRB.CheckedChanged += new EventHandler(configControlRB_CheckedChanged);
                        configControlRB.Name = t.Text + "_" + m.name + "_" + m.configs[i].name;
                        configPanel.Controls.Add(configControlRB);
                        break;

                    case "multi":
                        CheckBox configControlCB = new CheckBox();
                        configControlCB.AutoSize = true;
                        configControlCB.Location = new System.Drawing.Point(100, yPosition - 10);
                        //configControlCB.Name = "configControlCB";
                        configControlCB.Size = new System.Drawing.Size(150, 15);
                        configControlCB.TabIndex = 1;
                        configControlCB.TabStop = true;
                        configControlCB.Enabled = false;
                        configControlCB.CheckedChanged += new EventHandler(configControlCB_CheckedChanged);
                        configControlCB.Name = t.Text + "_" + m.name + "_" + m.configs[i].name;
                        configPanel.Controls.Add(configControlCB);
                        break;

                    case "value_enter":
                        TextBox configControlTB = new TextBox();
                        configControlTB.Text = m.configDefault;
                        configControlTB.AutoSize = true;
                        configControlTB.Location = new System.Drawing.Point(100, yPosition-10);
                        //configControlTB.Name = "configControlTB";
                        configControlTB.Size = new System.Drawing.Size(150, 15);
                        configControlTB.TabIndex = 1;
                        configControlTB.TabStop = true;
                        configControlTB.Enabled = false;
                        configControlTB.TextChanged += new EventHandler(configControlTB_TextChanged);
                        configControlTB.Name = t.Text + "_" + m.name + "_" + m.configs[i].name;
                        configControlTB.BackColor = Color.Green;
                        configPanel.Controls.Add(configControlTB);
                        break;
                }
            }

            //make modCheckBox
            CheckBox modCheckBox = new CheckBox();
            modCheckBox.AutoSize = true;
            modCheckBox.Location = new System.Drawing.Point(3, 3);
            //modCheckBox.Name = "modCheckBox";
            modCheckBox.Size = new System.Drawing.Size(49, 17);
            modCheckBox.TabIndex = 1;
            modCheckBox.Text = m.name;
            modCheckBox.UseVisualStyleBackColor = true;
            modCheckBox.CheckedChanged += new EventHandler(modCheckBox_CheckedChanged);

            //make mainPanel
            Panel mainPanel = new Panel();
            mainPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            mainPanel.Controls.Add(configPanel);
            mainPanel.Controls.Add(modCheckBox);
            int panelCountYLocation = 70 * (panelCount - 1);
            if (panelCount > 1)
            {
                panelCountYLocation = (panelCount - 1)*(t.Controls[0].Size.Height);
                panelCountYLocation = panelCountYLocation + 5;
            }
            mainPanel.Location = new System.Drawing.Point(5, panelCountYLocation+5);
            //mainPanel.Name = "mainPanel";
            mainPanel.Size = new System.Drawing.Size(this.Size.Width - 50, 20);
            mainPanel.TabIndex = 0;
            mainPanel.AutoSize = true;
            mainPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;

            //add them to everything
            mainPanel.Controls.Clear();
            mainPanel.Controls.Add(modCheckBox);
            mainPanel.Controls.Add(configPanel);
            t.Controls.Add(mainPanel);

        }

        void configControlTB_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            try
            {
                int temp = int.Parse(tb.Text);
                tb.BackColor = Color.Green;
                string modName = tb.Parent.Parent.Controls[0].Text;
                string catagoryName = tb.Parent.Parent.Parent.Text;
                foreach (Catagory c in parsedCatagoryList)
                {
                    foreach (Mod m in c.mods)
                    {
                        if (c.name.Equals(catagoryName) && m.name.Equals(modName))
                        {
                            foreach (Config cc in m.configs)
                            {
                                string configName = tb.Name.Split('_')[2];
                                if (configName.Equals(cc.name))
                                {
                                    cc.zipConfigFile = tb.Text;
                                }
                            }
                        }
                    }
                }
            }
            catch (FormatException)
            {
                tb.BackColor = Color.Red;
            }
        }

        void configControlCB_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            string modName = cb.Parent.Parent.Controls[0].Text;
            string catagoryName = cb.Parent.Parent.Parent.Text;
            foreach (Catagory c in parsedCatagoryList)
            {
                foreach (Mod m in c.mods)
                {
                    if (c.name.Equals(catagoryName) && m.name.Equals(modName))
                    {
                        foreach (Config cc in m.configs)
                        {
                            string configName = cb.Name.Split('_')[2];
                            if (configName.Equals(cc.name))
                            {
                                cc.configChecked = cb.Checked;
                            }
                        }
                    }
                }
            }
        }

        void configControlRB_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            string modName = rb.Parent.Parent.Controls[0].Text;
            string catagoryName = rb.Parent.Parent.Parent.Text;
            foreach (Catagory c in parsedCatagoryList)
            {
                foreach (Mod m in c.mods)
                {
                    if (c.name.Equals(catagoryName) && m.name.Equals(modName))
                    {
                        foreach (Config cc in m.configs)
                        {
                            cc.configChecked = false;
                        }
                        foreach (Config cc in m.configs)
                        {
                            string configName = rb.Name.Split('_')[2];
                            if (configName.Equals(cc.name))
                            {
                                cc.configChecked = rb.Checked;
                            }
                        }
                    }
                }
            }
        }

        void modCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //update the ui with the change
            CheckBox cb = (CheckBox)sender;
            Panel p = (Panel)cb.Parent;
            Panel innerPanel = (Panel)p.Controls[1];
            if (cb.Checked) innerPanel.BackColor = Color.BlanchedAlmond;
            else innerPanel.BackColor = SystemColors.Control;
            foreach (Control c in innerPanel.Controls)
            {
                c.Enabled = cb.Checked;
            }
            //update the memory database with the change
            foreach (Catagory c in parsedCatagoryList)
            {
                foreach (Mod m in c.mods)
                {
                    if (m.name.Equals(cb.Text))
                    {
                        m.modChecked = cb.Checked;
                    }
                }
            }
        }

        private void createModStructure()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("modInfoAlpha.xml");
            XmlNodeList modsList = doc.SelectNodes("//modInfoAlpha.xml/mods/mod");
            parsedModsList = new List<Mod>();
            foreach (XmlNode n in modsList)
            {
                Mod m = new Mod();
                foreach (XmlNode nn in n.ChildNodes)
                {
                    switch (nn.Name)
                    {
                        case "name":
                            m.name = nn.InnerText;
                            break;
                        case "version":
                            m.version = float.Parse(nn.InnerText);
                            break;
                        case "modzipfile":
                            m.modZipFile = nn.InnerText;
                            break;
                        case "modzipcrc":
                            m.crc = nn.InnerText;
                            break;
                        case "enabled":
                            m.enabled = bool.Parse(nn.InnerText);
                            break;
                        case "configselectiontype":
                            m.configType = nn.InnerText;
                            break;
                        case "configs":
                            //parse every config for that mod
                            foreach (XmlNode nnn in nn.ChildNodes)
                            {
                                Config c = new Config();
                                foreach (XmlNode nnnn in nnn.ChildNodes)
                                {
                                    switch (nnnn.Name)
                                    {
                                        case "name":
                                            c.name = nnnn.InnerText;
                                            break;
                                        case "configzipfile":
                                            c.zipConfigFile = nnnn.InnerText;
                                            break;
                                        case "configzipcrc":
                                            c.crc = nnnn.InnerText;
                                            break;
                                        case "configenabled":
                                            c.enabled = bool.Parse(nnn.InnerText);
                                            break;
                                    }
                                }
                                m.configs.Add(c);
                            }
                            break;
                    }
                }
                parsedModsList.Add(m);
            }
        }

        private void createModStructure2()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("modInfo.xml");
            XmlNodeList catagoryList = doc.SelectNodes("//modInfoAlpha.xml/catagories/catagory");
            parsedCatagoryList = new List<Catagory>();
            foreach (XmlNode nnnnn in catagoryList)
            {
                Catagory cat = new Catagory();
                foreach (XmlNode nnnnnn in nnnnn.ChildNodes)
                {
                    switch (nnnnnn.Name)
                    {
                        case "name":
                            cat.name = nnnnnn.InnerText;
                            break;
                        case "selectionType":
                            cat.selectionType = nnnnnn.InnerText;
                            break;
                        case "mods":
                            foreach (XmlNode n in nnnnnn.ChildNodes)
                            {
                                Mod m = new Mod();
                                foreach (XmlNode nn in n.ChildNodes)
                                {
                                    switch (nn.Name)
                                    {
                                        case "name":
                                            m.name = nn.InnerText;
                                            break;
                                        case "version":
                                            m.version = float.Parse(nn.InnerText);
                                            break;
                                        case "modzipfile":
                                            m.modZipFile = nn.InnerText;
                                            break;
                                        case "modzipcrc":
                                            m.crc = nn.InnerText;
                                            break;
                                        case "enabled":
                                            m.enabled = bool.Parse(nn.InnerText);
                                            break;
                                        case "configselectiontype":
                                            m.configType = nn.InnerText;
                                            break;
                                        case "configdefault":
                                            m.configDefault = nn.InnerText;
                                            break;
                                        case "configs":
                                            //parse every config for that mod
                                            foreach (XmlNode nnn in nn.ChildNodes)
                                            {
                                                Config c = new Config();
                                                foreach (XmlNode nnnn in nnn.ChildNodes)
                                                {
                                                    switch (nnnn.Name)
                                                    {
                                                        case "name":
                                                            c.name = nnnn.InnerText;
                                                            break;
                                                        case "configzipfile":
                                                            c.zipConfigFile = nnnn.InnerText;
                                                            break;
                                                        case "configzipcrc":
                                                            c.crc = nnnn.InnerText;
                                                            break;
                                                        case "configenabled":
                                                            c.enabled = bool.Parse(nnnn.InnerText);
                                                            break;
                                                    }
                                                }
                                                m.configs.Add(c);
                                            }
                                            break;
                                    }
                                }
                                cat.mods.Add(m);
                            }
                            break;
                    }
                }
                parsedCatagoryList.Add(cat);
            }
            
        }

        private void ModSelectionList_SizeChanged(object sender, EventArgs e)
        {
            continueButton.Location = new Point(this.Size.Width - 20 - continueButton.Size.Width, this.Size.Height - 39 - continueButton.Size.Height);
            cancelButton.Location = new Point(this.Size.Width - 20 - continueButton.Size.Width - 6 - cancelButton.Size.Width, this.Size.Height - 39 - continueButton.Size.Height);
            modTabGroups.Size = new Size(this.Size.Width - 20 - modTabGroups.Location.X, this.Size.Height - 72 - modTabGroups.Location.Y);
            label1.Text = "" + this.Size.Width + " x " + this.Size.Height;
            if (this.Size.Height < 250) this.Size = new Size(this.Size.Width, 250);
            if (this.Size.Width < 500) this.Size = new Size(500, this.Size.Height);
            foreach (TabPage t in modTabGroups.TabPages)
            {
                foreach (Control c in t.Controls)
                {
                    if (c is Panel)
                    {
                        Panel p = (Panel)c;
                        p.Size = new Size(this.Size.Width - 50, p.Size.Height);
                        foreach (Control cc in p.Controls)
                        {
                            if (cc is Panel)
                            {
                                Panel pp = (Panel)cc;
                                pp.Size = new Size(this.Size.Width - 60, pp.Size.Height);
                            }
                        }
                    }
                }
            }
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            cancel = true;
            this.Close();
        }
        
    }
}
