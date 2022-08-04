#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.ContentManager.PropertyGridItems;

namespace Exomia.Framework.ContentManager;

/// <summary>
///     Form for creating projects.
/// </summary>
partial class CreateProjectForm : Form
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CreateProjectForm" /> class.
    /// </summary>
    public CreateProjectForm()
    {
        InitializeComponent();
    }

    /// <summary>
    ///     Creates project file.
    /// </summary>
    /// <returns>
    ///     The new project file.
    /// </returns>
    public ProjectFile CreateProjectFile()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(locationTb.Text, nameTb.Text));
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }

        ProjectFile projectFile = new ProjectFile(nameTb.Text, directoryInfo.FullName)
        {
            Content = new ContentPropertyGridItem
            {
                Name            = "Content",
                TotalItems      = 0,
                ProjectName     = nameTb.Text,
                ProjectLocation = directoryInfo.FullName
            }
        };

        Json.Serialize(projectFile.FilePath, projectFile);

        DirectoryInfo dci = directoryInfo.CreateSubdirectory(projectFile.Content.Name);
        foreach (FileInfo file in dci.GetFiles())
        {
            file.Delete();
        }
        foreach (DirectoryInfo dir in dci.GetDirectories())
        {
            dir.Delete(true);
        }

        return projectFile;
    }

    private void button1_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(locationTb.Text) || string.IsNullOrEmpty(nameTb.Text)) { return; }

        FileInfo fileInfo = new FileInfo(Path.Combine(locationTb.Text, nameTb.Text, $"{nameTb.Text}.ecp"));
        if (fileInfo.Exists)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (
                MessageBox.Show(
                    // ReSharper disable once LocalizableElement
                    $"A project with the same name '{nameTb.Text}' already exists!\n\nDo you want to overwrite it?",
                    // ReSharper disable once LocalizableElement
                    "Project already exists!",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Error))
            {
                case DialogResult.Yes:
                    break;
                case DialogResult.None:
                case DialogResult.Cancel:
                    return;
                case DialogResult.No:
                    DialogResult = DialogResult.No;
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        DialogResult = DialogResult.OK;
    }

    private void button2_Click(object sender, EventArgs e)
    {
        using (FolderBrowserDialog dialog = new FolderBrowserDialog
               {
                   // ReSharper disable once LocalizableElement
                   Description            = "Project location for an \"exomia content project\" (.ecp)",
                   UseDescriptionForTitle = true,
                   AutoUpgradeEnabled     = true,
                   ShowNewFolderButton    = true
               })
        {
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                locationTb.Text = dialog.SelectedPath;
                button1.Enabled = !string.IsNullOrEmpty(locationTb.Text) && !string.IsNullOrEmpty(nameTb.Text);
            }
        }
    }

    private void nameTb_TextChanged(object sender, EventArgs e)
    {
        button1.Enabled = !string.IsNullOrEmpty(locationTb.Text) && !string.IsNullOrEmpty(nameTb.Text);
    }
}