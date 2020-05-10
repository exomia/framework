using System;
using System.ComponentModel;
using System.Windows.Forms;
using Exomia.Framework.ContentManager.Extensions;
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
                    panel1.InvokeIfRequired(x => x.Enabled = true);
                    treeView1.InvokeIfRequired(
                        x =>
                        {
                            x.Nodes.Clear();
                            var node = x.Nodes.Add("-ROOT-", "Content", 0, 0);
                            node.Tag =
                                new ContentPropertyGridItem(
                                    () => node.Text,
                                    () => "/",
                                    () => node.GetNodeCount(true),
                                    () => "C://.../test/test/test/test/test/test/test/build");
                            node.ContextMenuStrip = rootContextMenuStrip;
                        });
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

        #endregion

        #region PropertyGrid1

        #endregion

        #region TreeView1

        private const string FOLDER_KEY_PREFIX = "-folder-";

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
            if (e.Label == null) { return; }

            e.CancelEdit = true;

            bool Check(TreeNode node, Func<TreeNode, TreeNode> next)
            {
                while ((node = next(node)) != null)
                {
                    if (node.Text.Equals(e.Label ?? e.Node.Text, StringComparison.InvariantCultureIgnoreCase))
                    {
                        e.Node.BeginEdit();
                        SetStatusLabel(
                            StatusType.Error,
                            e.Node.Name.StartsWith(FOLDER_KEY_PREFIX)
                                ? "A folder with the same name '{0}' already exists in '{1}'"
                                : "A item with the same name '{0}' already exists in '{1}'",
                            e.Label ?? e.Node.Text, e.Node.Parent.FullPath);
                        return false;
                    }
                }
                return true;
            }

            if (e.Node.Level == 0 ||
                (Check(e.Node, n => n.PrevNode) &&
                 Check(e.Node, n => n.NextNode)))
            {
                string oldText = e.Node.Text;
                e.Node.Text = e.Label;

                propertyGrid1.InvokeIfRequired(
                    x =>
                    {
                        x.RefreshTabs(PropertyTabScope.Component);
                        x.Refresh();
                    });
                SetStatusLabel(
                    StatusType.Info,
                    e.Node.Level == 0
                        ? "The project '{0}' was successfully renamed to '{1}'."
                        : e.Node.Name.StartsWith(FOLDER_KEY_PREFIX)
                            ? "The folder '{0}' was successfully renamed to '{1}' under '{2}'"
                            : "The item '{0}' was successfully renamed to '{1}' under '{2}'",
                    oldText, e.Node.Text, e.Node.Parent?.FullPath);
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
                        x.SelectedNode.Remove();
                        propertyGrid1.InvokeIfRequired(p => p.SelectedObject = null);
                    }
                });
        }

        #endregion

        #region rootContextMenuStrip & folderContextMenuStrip

        private void folderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1.InvokeIfRequired(
                x =>
                {
                    var selectedNode = x.SelectedNode ?? x.TopNode;
                    if (selectedNode == null) { return; }
                    int selectedNodeCount = selectedNode.GetNodeCount(false);
                    var node = selectedNode.Nodes.Add(
                        $"{FOLDER_KEY_PREFIX}{selectedNodeCount}",
                        $"NewFolder{selectedNodeCount}", 1, 1);
                    node.Tag =
                        new FolderPropertyGridItem(
                            () => node.Text,
                            () => $"/{node.Parent.FullPath}",
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

        #endregion
    }
}