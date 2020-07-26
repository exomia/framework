#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using Exomia.Framework.ContentManager.Attributes;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Exomia.Framework.ContentManager.PropertyGridItems.Editor
{
    /// <summary>
    ///     Editor for folder name.
    /// </summary>
    public class FolderNameEditor : UITypeEditor
    {
        /// <inheritdoc />
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <inheritdoc />
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                Title = context.PropertyDescriptor.Attributes.OfType<FolderNameEditorTitleAttribute>()
                               ?.FirstOrDefault()
                               ?.Title ?? "Select a folder.",
                RestoreDirectory = true,
                IsFolderPicker   = true
            })
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    return dialog.FileName;
                }
            }
            return value;
        }
    }
}