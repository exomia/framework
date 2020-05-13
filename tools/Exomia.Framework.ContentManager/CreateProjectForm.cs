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
        ///     Initializes a new instance of the <see cref="CreateProjectForm" /> class.
        /// </summary>
        public CreateProjectForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}