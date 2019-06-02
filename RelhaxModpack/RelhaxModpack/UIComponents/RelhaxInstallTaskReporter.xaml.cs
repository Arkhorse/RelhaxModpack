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
    public enum TaskReportState
    {
        Inactive,
        Active,
        Complete,
        Error
    }

    /// <summary>
    /// Interaction logic for RelhaxInstallTaskReporter.xaml
    /// </summary>
    public partial class RelhaxInstallTaskReporter : UserControl
    {
        public RelhaxInstallTaskReporter()
        {
            InitializeComponent();
        }

        #region Properties
        private TaskReportState _reportState = TaskReportState.Inactive;
        public TaskReportState ReportState
        {
            get
            { return _reportState; }
            set
            {
                _reportState = value;
                if(value == TaskReportState.Active)
                {
                    IsEnabled = true;
                }
                else
                {
                    IsEnabled = false;
                    if(value == TaskReportState.Error)
                    {
                        Background = new SolidColorBrush(Colors.Red);
                    }
                    else if (value == TaskReportState.Complete)
                    {
                        Background = new SolidColorBrush(Colors.Green);
                    }
                }
            }
        }
        public bool IsSubProgressActive { get; set; } = false;
        public string TaskTitle { get; set; } = string.Empty;
        public string TaskText { get; set; } = string.Empty;
        public int TaskMinimum { get; set; } = 0;
        public int TaskMaximum { get; set; } = 100;
        public int TaskValue { get; set; } = 0;
        public int SubTaskMinimum { get; set; } = 0;
        public int SubTaskMaximum { get; set; } = 100;
        public int SubTaskValue { get; set; } = 0;
        #endregion

        #region Property changed code
        //https://stackoverflow.com/questions/34651123/wpf-binding-a-background-color-initializes-but-not-updating
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handle = PropertyChanged;
            if (handle != null)
                handle(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
