#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Windows.Forms;
using Exomia.Framework.ContentManager.Extensions;
using Exomia.Framework.ContentManager.Properties;

namespace Exomia.Framework.ContentManager
{
    /// <summary>
    ///     The application's main form.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MainForm" /> class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            SetProgressbarValue(true);
            SetStatusLabel(StatusType.Info, "Startup...");
            SetStatusLabel(StatusType.Info, "Startup finished...");
            SetProgressbarValue(false);
        }

        /// <summary>
        ///     Sets status label.
        /// </summary>
        /// <param name="statusType"> Type of the status. </param>
        /// <param name="text">       The text. </param>
        /// <param name="args">       A variable-length parameters list containing arguments. </param>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
        public void SetStatusLabel(StatusType statusType, string text, params object?[] args)
        {
            statusStrip1.InvokeIfRequired(
                x =>
                {
                    toolStripStatusLabel1.Image = statusType switch
                    {
                        StatusType.Warning => Resources
                            .StatusWarning_16x,
                        StatusType.Error => Resources
                            .StatusCriticalError_16x,
                        StatusType.Info => Resources
                            .StatusInformation_16x,
                        _ => throw new ArgumentOutOfRangeException(nameof(statusType), statusType, null)
                    };
                    toolStripStatusLabel1.Text = string.Format(text, args);
                });
        }

        /// <summary>
        ///     Sets progressbar value.
        /// </summary>
        /// <param name="state"> true to set progressbar to running state; false otherwise. </param>
        public void SetProgressbarValue(bool state)
        {
            statusStrip1.InvokeIfRequired(
                x =>
                {
                    toolStripProgressBar1.Style = state
                        ? ProgressBarStyle.Marquee
                        : ProgressBarStyle.Continuous;
                    toolStripProgressBar1.Value = 0;
                });
        }
    }
}