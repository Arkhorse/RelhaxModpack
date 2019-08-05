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
using System.Net;
using System.IO;
using System.Xml;

namespace RelhaxModpack.Windows
{
    /// <summary>
    /// Event argument passed back to the caller for when the developer selection window is closed
    /// </summary>
    public class DevleoperSelectionsClosedEWventArgs : EventArgs
    {
        /// <summary>
        /// Determines if a selection should be loaded (for example, if the user closed the window instead of selecting one)
        /// </summary>
        public bool LoadSelection = false;

        /// <summary>
        /// the name of the file to load from the online list of selection files
        /// </summary>
        public string FileToLoad = "";
    }

    /// <summary>
    /// The delegate callback for when the developer selections window is closed
    /// </summary>
    /// <param name="sender">The sender (this)</param>
    /// <param name="e">The arguments</param>
    public delegate void DeveloperSelectionsClosedDelagate(object sender, DevleoperSelectionsClosedEWventArgs e);

    /// <summary>
    /// Interaction logic for DeveloperSelectionsViewer.xaml
    /// </summary>
    public partial class DeveloperSelectionsViewer : RelhaxWindow
    {
        //public
        /// <summary>
        /// Fires when the developer selection window is closed
        /// </summary>
        public event DeveloperSelectionsClosedDelagate OnDeveloperSelectionsClosed;

        //private
        //init it to false so that it only will get changed to true at that one point when it works
        private bool LoadSelection = false;
        private string FileToLoad = "";
        private WebClient client;

        /// <summary>
        /// Create an instance of the DeveloperSelectionsViewer window
        /// </summary>
        public DeveloperSelectionsViewer()
        {
            InitializeComponent();
        }

        private async void OnApplicationLoading(object sender, RoutedEventArgs e)
        {
            DeveloperSelectionsContinueButton.IsEnabled = false;
            string selectionsXMlString = string.Empty;
            using (client = new WebClient())
            {
                try
                {
                    selectionsXMlString = await client.DownloadStringTaskAsync(Settings.SelectionsRoot + Settings.SelectionsXml);
                }
                catch(Exception ex)
                {
                    Logging.Exception(ex.ToString());
                    MessageBox.Show(Translations.GetTranslatedString("failedToParseSelections"));
                    Close();
                }
            }
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(selectionsXMlString);
            }
            catch (XmlException xmlex)
            {
                Logging.Exception(xmlex.ToString());
                MessageBox.Show(Translations.GetTranslatedString("failedToParseSelections"));
                return;
            }

            //load selections into stackpanel
            XmlNodeList selectionsList = XmlUtils.GetXmlNodesFromXPath(doc, "//selections/selection");
            if(selectionsList == null || selectionsList.Count == 0)
            {
                Logging.Error("selectionsList is null or count is 0 after download");
                MessageBox.Show(Translations.GetTranslatedString("failedToParseSelections"));
                return;
            }
            bool firstOne = true;
            foreach (XmlNode node in selectionsList)
            {
                DeveloperSelectionsStackPanel.Children.Add(new RadioButton()
                {
                    Content = node.Attributes["displayName"],
                    ToolTip = Translations.GetTranslatedString("lastModified") + ": " + node.Attributes["lastModified"],
                    Tag = node.InnerText,
                    IsChecked = firstOne
                });
                firstOne = false;
            }

            //enable the button to select them
            DeveloperSelectionsContinueButton.IsEnabled = true;
        }

        private void OnApplicationClosed(object sender, EventArgs e)
        {
            if (OnDeveloperSelectionsClosed != null)
            {
                //only perform selection logic if the user selected to load one
                if(!LoadSelection)
                {
                    FileToLoad = string.Empty;
                }
                else if (!FileToLoad.Equals("LOCAL"))
                {
                    //checkif a radio button is selected. to do that, set LoadSelection to false. forces a button to set it to true
                    LoadSelection = false;
                    foreach (RadioButton button in DeveloperSelectionsStackPanel.Children)
                    {
                        if ((bool)button.IsChecked)
                        {
                            LoadSelection = true;
                            FileToLoad = (string)button.Tag;
                            break;
                        }
                    }
                }
                OnDeveloperSelectionsClosed(this, new DevleoperSelectionsClosedEWventArgs()
                {
                    LoadSelection = LoadSelection,
                    FileToLoad = FileToLoad
                });
            }
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            LoadSelection = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            LoadSelection = false;
            FileToLoad = string.Empty;
            Close();
        }

        private void LocalFile_Click(object sender, RoutedEventArgs e)
        {
            LoadSelection = true;
            FileToLoad = "LOCAL";
            Close();
        }
    }
}
