#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Windows.Forms;

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
        ///     Initializes a new instance of the <see cref="CreateProjectForm" /> class.
        /// </summary>
        public CreateProjectForm()
        {
            InitializeComponent();
            locationTb.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(locationTb.Text)) { return; }
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
                FileName                     = "content.ecp"
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    locationTb.Text = dialog.FileName;
                }
            }
        }
    }
}