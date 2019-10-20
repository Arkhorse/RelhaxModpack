﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RelhaxModpack.UIComponents
{
    /// <summary>
    /// Interaction logic for RelhaxWPFRadioButton.xaml
    /// </summary>
    public partial class RelhaxWPFRadioButton : RadioButton, IPackageUIComponent, INotifyPropertyChanged
    {
        /// <summary>
        /// Create an instance of the RelhaxWPFRadioButton class
        /// </summary>
        public RelhaxWPFRadioButton()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The package associated with this UI component
        /// </summary>
        public SelectablePackage Package { get; set; }

        /// <summary>
        /// Change any UI parent class properties that depends on the enabled SelectablePackage
        /// </summary>
        /// <param name="Enabled">The value from the SelectablePackage</param>
        public void OnEnabledChanged(bool Enabled)
        {
            IsEnabled = Enabled;
        }

        /// <summary>
        /// Change any UI parent class properties that depends on the checked SelectablePackage
        /// </summary>
        /// <param name="Checked">The value from the SelectablePackage</param>
        public void OnCheckedChanged(bool Checked)
        {
            IsChecked = Checked;
        }

        /// <summary>
        /// Set the color of the RadioButton Foreground property
        /// </summary>
        public Brush TextColor
        {
            get
            { return Foreground; }
            set
            { Foreground = value; }
        }

        /// <summary>
        /// Set the brush of the RadioButton Panel Background property 
        /// </summary>
        public Brush PanelColor
        {
            get
            {
                return Package.ParentBorder == null ? null : Package.ParentBorder.Background;
            }
            set
            {
                if (Package.ParentBorder != null)
                    Package.ParentBorder.Background = value;
            }
        }

        #region Data UI Binding

        private Color _DisabledColor = Colors.DarkGray;

        /// <summary>
        /// Set the value of the disabled component color
        /// </summary>
        public Color DisabledColor
        {
            get
            {
                return _DisabledColor;
            }
            set
            {
                _DisabledColor = value;
                OnPropertyChanged(nameof(DisabledColor));
            }
        }

        private Visibility _PopularModVisability = Visibility.Hidden;

        /// <summary>
        /// Set the visibility of the popular mod icon
        /// </summary>
        public Visibility PopularModVisability
        {
            get { return _PopularModVisability; }
            set
            {
                _PopularModVisability = value;
                OnPropertyChanged(nameof(PopularModVisability));
            }
        }

        private Visibility _GreyAreaVisability = Visibility.Hidden;

        /// <summary>
        /// Set visibility of the Grey area icon
        /// </summary>
        public Visibility GreyAreaVisability
        {
            get { return _GreyAreaVisability; }
            set
            {
                _GreyAreaVisability = value;
                OnPropertyChanged(nameof(GreyAreaVisability));
            }
        }

        //https://stackoverflow.com/questions/34651123/wpf-binding-a-background-color-initializes-but-not-updating
        /// <summary>
        /// Event to trigger when an internal property is changed. It forces a UI update
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Method to invoke the PropertyChanged event to update the UI
        /// </summary>
        /// <param name="propertyName">The name of the property that changed, to update it's UI binding</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
