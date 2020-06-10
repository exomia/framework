#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Exomia.Framework.ContentManager.Editor;
using Exomia.Framework.ContentManager.Extensions;
using Exomia.Framework.ContentManager.Fonts;
using Exomia.Framework.ContentManager.PropertyGridItems;

namespace Exomia.Framework.ContentManager
{
    /// <content>
    ///     The application's main form.
    /// </content>
    partial class MainForm
    {
        #region menuStrip1

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (CreateProjectForm createProjectForm = new CreateProjectForm())
            {
                if (createProjectForm.ShowDialog() == DialogResult.OK)
                {
                    if (_projectFile != null)
                    {
                        using (FileStream fs = File.Create(_projectFile.Location))
                        {
                            _formatter.Serialize(fs, _projectFile);
                            _projectFile = null;
                        }
                    }

                    _projectFile = createProjectForm.CreateProjectFile();

                    panel1.InvokeIfRequired(x => x.Enabled = true);

                    treeView1.InvokeIfRequired(
                        x =>
                        {
                            x.Nodes.Clear();

                            var node = x.Nodes.Add(ROOT_KEY_PREFIX, _projectFile.Content, 0, 0);
                            node.Tag = new ContentPropertyGridItem(
                                () => node.Text,
                                Provider.Static(string.Empty),
                                () => node.GetNodeCount(true),

                                // ReSharper disable once AccessToDisposedClosure
                                _projectFile.Name,

                                // ReSharper disable once AccessToDisposedClosure
                                _projectFile.Location);

                            node.ContextMenuStrip = rootContextMenuStrip;
                        });

                    menuStrip1.InvokeIfRequired(
                        x =>
                        {
                            ForAll(
                                i => i.Enabled = true, buildToolStripMenuItem, editToolStripMenuItem,
                                closeToolStripMenuItem);
                        });

                    SetStatusLabel(
                        StatusType.Info, "Project '{0}' created under {1}",
                        Path.GetFileNameWithoutExtension(_projectFile.Name),
                        Path.GetDirectoryName(_projectFile.Location));
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog
            {
                Title                        = "Open an existing \"exomia content project\" (.ecp)",
                Filter                       = "exomia content project (*.ecp)|*.ecp",
                DefaultExt                   = "ecp",
                AddExtension                 = true,
                CheckFileExists              = true,
                AutoUpgradeEnabled           = true,
                DereferenceLinks             = true,
                RestoreDirectory             = true,
                ShowHelp                     = false,
                Multiselect                  = false,
                SupportMultiDottedExtensions = true
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK) { }
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            menuStrip1.InvokeIfRequired(
                x =>
                {
                    ForAll(
                        i => i.Enabled = false, buildToolStripMenuItem, editToolStripMenuItem, closeToolStripMenuItem);
                });

            treeView1.InvokeIfRequired(
                x =>
                {
                    x.Nodes.Clear();
                });

            panel1.InvokeIfRequired(x => x.Enabled = false);

            SetStatusLabel(StatusType.Info, "Project closed...");
        }

        private async void buildToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            await BuildAsync();
        }

        private void cancelBuildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CancelBuild();
        }

        #endregion

        #region PropertyGrid1

        #endregion

        #region TreeView1

        private const string ROOT_KEY_PREFIX   = "-ROOT-";
        private const string FOLDER_KEY_PREFIX = "-folder-";
        private const string FONT_KEY_PREFIX   = "-font-";

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView1.SelectedNode = e.Node;
        }

        private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e) { }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            propertyGrid1.InvokeIfRequired(
                x =>
                {
                    x.SelectedObject = null;
                    if (e.Node.Tag is PropertyGridItem item)
                    {
                        x.SelectedObject = item;
                    }
                });
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Name.StartsWith(FOLDER_KEY_PREFIX))
            {
                e.Node.SelectedImageIndex = e.Node.ImageIndex = 2;
            }
        }

        private void treeView1_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Name.StartsWith(FOLDER_KEY_PREFIX))
            {
                e.Node.SelectedImageIndex = e.Node.ImageIndex = 1;
            }
        }

        private void treeView1_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e) { }

        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label == null)
            {
                treeView1.BeginInvoke(new MethodInvoker(treeView1.Sort));
                return;
            }

            string label = e.Label;

            if (e.Node.Name.StartsWith(FONT_KEY_PREFIX))
            {
                label = Path.ChangeExtension(label, ".fnt");
            }

            e.CancelEdit = true;

            bool Check(TreeNode node, Func<TreeNode, TreeNode> next)
            {
                while ((node = next(node)) != null)
                {
                    if (node.Text.Equals(label, StringComparison.InvariantCultureIgnoreCase))
                    {
                        e.Node.BeginEdit();
                        SetStatusLabel(
                            StatusType.Error,
                            e.Node.Name.StartsWith(FOLDER_KEY_PREFIX)
                                ? "A folder with the same name '{0}' already exists in '{1}'"
                                : e.Node.Name.StartsWith(FONT_KEY_PREFIX)
                                    ? "A font with the same name '{0}' already exists in '{1}'"
                                    : "A item with the same name '{0}' already exists in '{1}'",
                            label, e.Node.Parent.FullPath);
                        return false;
                    }
                }
                return true;
            }

            if (e.Node.Level == 0 ||
                Check(e.Node, n => n.PrevNode) &&
                Check(e.Node, n => n.NextNode))
            {
                string oldText = e.Node.Text;
                e.Node.Text = label;

                if (e.Node.Level == 0)
                {
                    _projectFile!.Content = label;

                    Directory.Move(
                        Path.Combine(_projectFile!.Location, oldText),
                        Path.Combine(_projectFile!.Location, label));
                }
                else
                {
                    Directory.Move(
                        Path.Combine(_projectFile!.Location, e.Node.Parent.FullPath, oldText),
                        Path.Combine(_projectFile!.Location, e.Node.Parent.FullPath, label));
                }

                propertyGrid1.InvokeIfRequired(
                    x =>
                    {
                        x.RefreshTabs(PropertyTabScope.Component);
                        x.Refresh();
                    });

                SetStatusLabel(
                    StatusType.Info,
                    e.Node.Level == 0
                        ? "'{0}' was successfully renamed to '{1}'."
                        : e.Node.Name.StartsWith(FOLDER_KEY_PREFIX)
                            ? "The folder '{0}' was successfully renamed to '{1}' under '{2}'"
                            : e.Node.Name.StartsWith(FONT_KEY_PREFIX)
                                ? "The font '{0}' was successfully renamed to '{1}' under '{2}'"
                                : "The item '{0}' was successfully renamed to '{1}' under '{2}'",
                    oldText, e.Node.Text, e.Node.Parent?.FullPath);

                treeView1.BeginInvoke(new MethodInvoker(treeView1.Sort));
            }
        }

        private void treeView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                treeView1_RemoveSelectedNode();
            }
        }

        private void treeView1_RemoveSelectedNode()
        {
            treeView1.InvokeIfRequired(
                x =>
                {
                    if (x.SelectedNode.Level > 0)
                    {
                        FileSystemInfo fileSystemInfo = x.SelectedNode.Tag switch
                        {
                            ItemPropertyGridItem i => new FileInfo(
                                Path.Combine(
                                    _projectFile!.Location, i.VirtualPath, i.Name)),
                            FolderPropertyGridItem f => new DirectoryInfo(
                                Path.Combine(
                                    _projectFile!.Location, f.VirtualPath, f.Name)),
                            _ => throw new InvalidCastException()
                        };

                        if (fileSystemInfo.Exists)
                        {
                            fileSystemInfo.Delete();
                        }

                        x.SelectedNode.Remove();
                        propertyGrid1.InvokeIfRequired(p => p.SelectedObject = null);
                    }
                });
        }

        #endregion

        #region rootContextMenuStrip & folderContextMenuStrip

        private void addFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1.InvokeIfRequired(
                x =>
                {
                    var selectedNode = x.SelectedNode ?? x.TopNode;
                    if (selectedNode == null) { return; }
                    int selectedNodeCount = selectedNode.GetNodeCount(false);

                    DirectoryInfo di = new DirectoryInfo(
                        Path.Combine(_projectFile!.Location, selectedNode.FullPath, $"NewFolder{selectedNodeCount}"));
                    if (!di.Exists)
                    {
                        di.Create();
                    }

                    var node = selectedNode.Nodes.Add($"{FOLDER_KEY_PREFIX}{selectedNodeCount}", di.Name, 1, 1);
                    node.Tag =
                        new FolderPropertyGridItem(
                            () => node.Text,
                            () => node.Parent.FullPath,
                            () => node.GetNodeCount(true));
                    node.ContextMenuStrip = folderContextMenuStrip;
                    selectedNode.Expand();
                    treeView1.SelectedNode = node;
                    node.BeginEdit();
                });
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1_RemoveSelectedNode();
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1.InvokeIfRequired(x => x.SelectedNode.BeginEdit());
        }

        private void addFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1.InvokeIfRequired(
                x =>
                {
                    var selectedNode = x.SelectedNode ?? x.TopNode;
                    if (selectedNode == null) { return; }
                    int selectedNodeCount = selectedNode.GetNodeCount(false);

                    using (var jsonEditorForm = new JsonEditorForm(
                        new FontDescription
                        {
                            Name     = "Arial",
                            Chars    = "32-126,128,130-140,142,145-156,158-255",
                            Size     = 12,
                            IsBold   = false,
                            AA       = true,
                            IsItalic = false
                        }) { Text = "Add Font..." })
                    {
                        if (jsonEditorForm.ShowDialog() != DialogResult.OK)
                        {
                            SetStatusLabel(StatusType.Error, "The font is not added to the project!");
                            return;
                        }

                        int i = 0;
                        if (jsonEditorForm.Deserialize<FontDescription>(out var fntDescription))
                        {
                            string fntFilePath;
                            while (File.Exists(
                                fntFilePath = Path.Combine(
                                    _projectFile!.Location, selectedNode.FullPath,
                                    $"{fntDescription.Name}_{fntDescription.Size}{(i++ == 0 ? string.Empty : "_" + (i - 1))}.fnt"))
                            ) { }

                            jsonEditorForm.Save(fntFilePath);

                            var node = selectedNode.Nodes.Add(
                                $"{FONT_KEY_PREFIX}{selectedNodeCount}",
                                Path.GetFileName(fntFilePath), 4, 4);
                            node.Tag = new ItemPropertyGridItem(
                                () => node.Text,
                                () => node.Parent.FullPath);
                            selectedNode.Expand();
                            treeView1.SelectedNode = node;
                            node.BeginEdit();
                        }
                    }
                });
        }

        #endregion
    }
}