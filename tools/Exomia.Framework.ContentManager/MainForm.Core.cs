#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.IO;
using System.Windows.Forms;
using Exomia.Framework.ContentManager.Editor;
using Exomia.Framework.ContentManager.Extensions;
using Exomia.Framework.ContentManager.Fonts;
using Exomia.Framework.ContentManager.PropertyGridItems;

namespace Exomia.Framework.ContentManager
{
    partial class MainForm
    {
        private void Save()
        {
            if (_projectFile != null)
            {
                Json.Serialize(_projectFile.FilePath, _projectFile);
            }
        }

        private TreeNode? GetNodeFromPath(TreeNode node, string path)
        {
            if (node.FullPath == path)
            {
                return node;
            }
            if (node.Nodes.Count > 0)
            {
                foreach (TreeNode tn in node.Nodes)
                {
                    TreeNode? foundNode = GetNodeFromPath(tn, path);
                    if (foundNode != null)
                    {
                        return foundNode;
                    }
                }
            }
            return null;
        }

        private void treeView1_RemoveNode(TreeNode node)
        {
            if (node.Level > 0)
            {
                switch (node.Tag)
                {
                    case ItemPropertyGridItem i:
                        new FileInfo(
                            Path.Combine(
                                _projectFile!.Location, i.VirtualPath, i.Name)).DeleteIfExists();
                        break;
                    case FolderPropertyGridItem f:
                        new DirectoryInfo(
                            Path.Combine(
                                _projectFile!.Location, f.VirtualPath, f.Name)).DeleteIfExists();
                        foreach (TreeNode childNodes in node.Nodes)
                        {
                            treeView1_RemoveNode(childNodes);
                        }
                        break;
                    default: throw new InvalidCastException();
                }

                if (_projectFile.Resources.Remove((PropertyGridItem)node.Tag))
                {
                    if (node.Parent.Tag is FolderPropertyGridItem folderPropertyGridItem)
                    {
                        folderPropertyGridItem.TotalItems--;
                    }

                    node.Remove();
                }
            }
        }

        private void treeView1_RemoveSelectedNode()
        {
            treeView1.InvokeIfRequired(
                x =>
                {
                    treeView1_RemoveNode(x.SelectedNode);
                });
            propertyGrid1.InvokeIfRequired(p => p.SelectedObject = null);
        }

        private void EditItem(TreeNode node)
        {
            if (node == null) { return; }
            if (node.Tag is ItemPropertyGridItem item)
            {
                if (Path.GetExtension(item.Name) == ".fnt")
                {
                    FontDescription? description = Json.Deserialize<FontDescription>(
                        Path.Combine(_projectFile!.Location, item.VirtualPath, item.Name));
                    if (description == null) { return; }

                    using (var jsonEditorForm = new JsonEditorForm(description) { Text = $"Edit font '{item.Name}'" })
                    {
                        if (jsonEditorForm.ShowDialog() != DialogResult.OK)
                        {
                            SetStatusLabel(StatusType.Error, "The font was not edited!");
                            return;
                        }
                        jsonEditorForm.Save(Path.Combine(_projectFile!.Location, item.VirtualPath, item.Name));
                    }
                }
            }
        }
    }
}