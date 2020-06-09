#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace Exomia.Framework.ContentManager.Editor
{
    /// <content>
    ///     Form for editing the json. This class cannot be inherited.
    /// </content>
    public sealed partial class JsonEditorForm : Form
    {
        /// <summary>
        ///     Initializes a new instance of the &lt;see cref="JsonEditorForm&lt;T&gt;"/&gt; class.
        /// </summary>
        public JsonEditorForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsonEditorForm" /> class.
        /// </summary>
        /// <param name="json"> The JSON. </param>
        public JsonEditorForm(object json)
            : this()
        {
            fastColoredTextBox1.Text = JsonSerializer.Serialize(
                json, new JsonSerializerOptions { WriteIndented = true });
        }

        /// <summary>
        ///     Deserialize this object to the given type.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="obj"> [out] The object. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        public bool Deserialize<T>(out T obj)
        {
            try
            {
                obj = JsonSerializer.Deserialize<T>(fastColoredTextBox1.Text);
                return true;
            }
            catch
            {
                obj = default!;
                return false;
            }
        }

        /// <summary>
        ///     Saves the content to the given file.
        /// </summary>
        /// <param name="filePath"> The file path to save. </param>
        public void Save(string filePath)
        {
            fastColoredTextBox1.SaveToFile(filePath, Encoding.UTF8);
        }

        private void JsonEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                JsonDocument.Parse(fastColoredTextBox1.Text);
                DialogResult = DialogResult.OK;
            }
            catch
            {
                if (MessageBox.Show(
                        "The json is invalid!\nClose anyway?", "Invalid JSON!", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Error) ==
                    DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}