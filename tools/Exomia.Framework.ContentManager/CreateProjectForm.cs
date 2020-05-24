#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Exomia.Framework.ContentManager
{
    /// <summary>
    ///     Form for creating projects.
    /// </summary>
    public partial class CreateProjectForm : Form
    {
        /// <summary>
        ///     Gets the name of the project.
        /// </summary>
        /// <value>
        ///     The name of the project.
        /// </value>
        public string ProjectName
        {
            get { return projNameTb.Text; }
        }

        /// <summary>
        ///     Gets the project location.
        /// </summary>
        /// <value>
        ///     The project location.
        /// </value>
        public string ProjectLocation
        {
            get { return locationTb.Text; }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CreateProjectForm" /> class.
        /// </summary>
        public CreateProjectForm()
        {
            InitializeComponent();
            locationTb.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(locationTb.Text) || string.IsNullOrEmpty(projNameTb.Text)) { return; }
            DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title          = "Create an \"exomia content project\" (.ecp) within selected location.",
                Multiselect    = false
            })
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    locationTb.Text = dialog.FileName;
                }
            }
        }
    }
}