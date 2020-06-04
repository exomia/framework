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
        ///     Gets the project location.
        /// </summary>
        /// <value>
        ///     The project location.
        /// </value>
        public string OutputFolder
        {
            get { return outputTb.Text; }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CreateProjectForm" /> class.
        /// </summary>
        public CreateProjectForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(locationTb.Text) || string.IsNullOrEmpty(outputTb.Text)) { return; }
            DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog
            {
                Title                        = "Create an \"exomia content project\" (.ecp)",
                Filter                       = "exomia content project (*.ecp)|*.ecp",
                DefaultExt                   = "ecp",
                AddExtension                 = true,
                AutoUpgradeEnabled           = true,
                DereferenceLinks             = true,
                RestoreDirectory             = true,
                ShowHelp                     = false,
                SupportMultiDottedExtensions = true,
                FileName                     = "exomia_content_project.ecp"
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    locationTb.Text = dialog.FileName;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                Title = "Build Output Folder", RestoreDirectory = true, IsFolderPicker = true
            })
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    outputTb.Text = dialog.FileName;
                }
            }
        }
    }
}