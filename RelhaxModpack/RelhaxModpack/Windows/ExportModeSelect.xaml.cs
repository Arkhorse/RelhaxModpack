﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using static RelhaxModpack.Utils;

namespace RelhaxModpack.Windows
{
    /// <summary>
    /// Interaction logic for ExportModeSelect.xaml
    /// </summary>
    public partial class ExportModeSelect : RelhaxWindow
    {

        private List<VersionInfos> VersionInfosList = new List<VersionInfos>();

        public VersionInfos SelectedVersionInfo { get; private set; }

        public ExportModeSelect()
        {
            InitializeComponent();
        }

        private void RelhaxWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Logging.Debug("loading supported_clients from zip file");

            //parse each online folder to list type string
            VersionInfosList.Clear();
            string xmlString = GetStringFromZip(Settings.ManagerInfoDatFile, Settings.SupportedClients);
            XmlNodeList supportedClients = XMLUtils.GetXMLNodesFromXPath(xmlString, "//versions/version", Settings.SupportedClients);
            VersionInfosList = new List<VersionInfos>();
            foreach (XmlNode node in supportedClients)
            {
                VersionInfos newVersionInfo = new VersionInfos()
                {
                    WoTOnlineFolderVersion = node.Attributes["folder"].Value,
                    WoTClientVersion = node.InnerText
                };
                VersionInfosList.Add(newVersionInfo);
            }

            //load them into the list with tag holding info
            ExportSelectVersionPanel.Children.Clear();
            foreach (VersionInfos versionInfo in VersionInfosList)
            {
                RadioButton button = new RadioButton()
                {
                    Tag = versionInfo,
                    Content = string.Format("{0} = {1}, {2} = {3}", Translations.GetTranslatedString("ExportModeMajorVersion"), versionInfo.WoTOnlineFolderVersion,
                        Translations.GetTranslatedString("ExportModeMinorVersion"), versionInfo.WoTClientVersion)
                };
                ExportSelectVersionPanel.Children.Add(button);
            }

            //select the first one
            (ExportSelectVersionPanel.Children[0] as RadioButton).IsChecked = true;
        }

        private void ExportCancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ExportContinueButton_Click(object sender, RoutedEventArgs e)
        {
            //find the one that is selected
            foreach(RadioButton button in ExportSelectVersionPanel.Children)
            {
                if((bool)button.IsChecked)
                {
                    SelectedVersionInfo = (VersionInfos)button.Tag;
                    DialogResult = true;
                    Close();
                }
            }
        }
    }
}
