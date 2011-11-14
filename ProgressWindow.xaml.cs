// Filename:    ProgressWindow.xaml.cs
// Description: ProgressWindow implements a basic progress window which
//              supports a cancel button.
// 2007-01-22 nschan Initial revision.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace kazOilMap
{
    /// <summary>
    /// ProgressWindow implements a basic progress window that
    /// supports a Cancel button.
    /// </summary>
    public sealed partial class ProgressWindow : System.Windows.Window
    {
        #region Events
        /// <summary>
        /// Event to notify that the Cancel button has been pressed.
        /// </summary>
        public event CancelEventHandler Cancel;
        #endregion Events

        #region Private fields
        private string progressText;
        private double progressValue;
        #endregion Private fields

        #region Constructor
        /// <summary>
        /// Constructor for ProgressWindow class.
        /// </summary>
        public ProgressWindow()
        {
            InitializeComponent();
        }
        #endregion Constructor

        #region Properties
        /// <summary>
        /// Specify the text to display in the progress window.
        /// </summary>
        public string ProgressText
        {
            get { return this.progressText; }
            set
            {
                this.progressText = value;
                this.label1.Content = this.progressText;
            }
        }

        /// <summary>
        /// Specify the value of the progress bar.
        /// </summary>
        public double ProgressValue
        {
            get { return this.progressValue; }
            set
            {
                this.progressValue = value;
                this.progressBar1.Value = this.progressValue;
            }
        }
        #endregion Properties

        #region Event handling
        /// <summary>
        /// Handle the Click event for the Cancel button.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            CancelEventHandler evh = this.Cancel;
            if ( evh != null )
            {
                // Invoke the event handler.
                CancelEventArgs cancelArgs = new CancelEventArgs();
                evh(this, cancelArgs);

                // Close the window if cancel is set.
                if ( cancelArgs.Cancel )
                    this.Close();
            }
        }
        #endregion Event handling
    }
}

// END