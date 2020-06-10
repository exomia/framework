#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Exomia.Framework.ContentManager.Extensions;
using Exomia.Framework.ContentManager.IO;
using Exomia.Framework.ContentManager.PropertyGridItems;

namespace Exomia.Framework.ContentManager
{
    partial class MainForm
    {
        private static readonly (Func<ItemPropertyGridItem, bool>, string)[] s_checks =
        {
            (i => i.BuildAction == BuildAction.Ignore, "ignored during build!"),
            (i => i.Importer == null, "no importer set!"), (i => i.Exporter == null, "no exporter set!")
        };

        private int _build;

        private CancellationTokenSource? _cancellationTokenSource;

        private void CancelBuild()
        {
            if (Interlocked.CompareExchange(ref _build, 1, 1) == 1)
            {
                _cancellationTokenSource?.Cancel();
                menuStrip1.InvokeIfRequired(m => cancelBuildToolStripMenuItem.Enabled = false);
            }
        }

        private void WriteLineMessages(IEnumerable<(string, object?[])> messages)
        {
            foreach (var (text, args) in messages)
            {
                WriteLine($"\t> {text}", args);
            }
        }

        private async Task<(int succeeded, int skipped, int failed)> BuildAsync(CancellationToken cancellationToken)
        {
            int succeeded = 0;
            int skipped   = 0;
            int failed    = 0;

            var rootNode = treeView1.Nodes[ROOT_KEY_PREFIX];

            async Task ForTreeNode(TreeNode node, ContentPropertyGridItem contentPropertyGridItem)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (node.Tag is ItemPropertyGridItem gridItem)
                {
                    void SkipWithMessage(string msg)
                    {
                        WriteLine(
                            $"skipping item {{0}}! Reason: {msg}",
                            Path.Combine(gridItem.VirtualPath, gridItem.Name));
                        Interlocked.Increment(ref skipped);
                    }

                    foreach (var (check, msg) in s_checks)
                    {
                        if (check(gridItem))
                        {
                            SkipWithMessage(msg);
                            return;
                        }
                    }

                    WriteLine(
                        "Import item {0}...",
                        Path.Combine(gridItem.VirtualPath, gridItem.Name));

                    ImporterContext importerContext = new ImporterContext(
                        gridItem.Name!, gridItem.VirtualPath!);

                    object? obj;
                    using (FileStream fs = new FileStream(
                        Path.Combine(_projectFile!.Location, gridItem.VirtualPath, gridItem.Name),
                        FileMode.Open, FileAccess.Read))
                    {
                        obj = await gridItem.Importer!.ImportAsync(fs, importerContext, cancellationToken);
                    }

                    if (cancellationToken.IsCancellationRequested)
                    {
                        WriteLine("\nBuild canceled!");
                        return;
                    }

                    if (obj == null)
                    {
                        WriteLine("\t{0:red}", "failed with messages");
                        WriteLineMessages(importerContext.Messages);
                        Interlocked.Increment(ref failed);
                        return;
                    }
                    WriteLineMessages(importerContext.Messages);

                    WriteLine(
                        "Export item {0}...",
                        Path.Combine(gridItem.VirtualPath, gridItem.Name));

                    ExporterContext exporterContext = new ExporterContext(
                        gridItem.Name!,
                        gridItem.VirtualPath!,
                        contentPropertyGridItem.OutputFolder ??
                        contentPropertyGridItem.ProjectLocation!);

                    if (!gridItem.Exporter!.Export(obj, exporterContext))
                    {
                        WriteLine("\tfailed with messages:");
                        WriteLineMessages(exporterContext.Messages);
                        Interlocked.Increment(ref failed);
                        return;
                    }

                    WriteLineMessages(exporterContext.Messages);
                    Interlocked.Increment(ref succeeded);
                    return;
                }

                foreach (TreeNode treeNode in node.Nodes)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                    await ForTreeNode(treeNode, contentPropertyGridItem);
                }
            }

            await ForTreeNode(rootNode, (ContentPropertyGridItem)rootNode.Tag);

            return (succeeded, skipped, failed);
        }

        private async Task BuildAsync()
        {
            if (Interlocked.CompareExchange(ref _build, 1, 0) == 1)
            {
                SetStatusLabel(StatusType.Error, "A build process is already running!");
                return;
            }

            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();

            menuStrip1.InvokeIfRequired(
                m =>
                {
                    ForAll(
                        i => i.Enabled = false,
                        buildToolStripMenuItem1, cleanToolStripMenuItem,
                        rebuildToolStripMenuItem);
                    cancelBuildToolStripMenuItem.Enabled = true;
                    cancelBuildToolStripMenuItem.Visible = true;
                });

            SetProgressbarValue(true);
            SetStatusLabel(
                StatusType.Info,
                "Build started {0}",
                DateTime.Now.ToString(
                    "dd/MMM/yyyy HH:mm:ss \"UTC\"z", CultureInfo.InvariantCulture));
            Clear();
            WriteLine(
                "Build started {0}",
                DateTime.Now.ToString(
                    "dd/MMM/yyyy HH:mm:ss \"UTC\"z", CultureInfo.InvariantCulture));

            splitContainer1.Panel1.InvokeIfRequired(t => t.Enabled = false);

            CancellationToken cancellationToken = _cancellationTokenSource.Token;
            await Task.Run(
                          async () =>
                          {
                              Stopwatch stopwatch = Stopwatch.StartNew();
                              (int succeeded, int skipped, int failed) = await BuildAsync(cancellationToken);
                              stopwatch.Stop();

                              WriteLine(
                                  "\nBuild{3:Black}! {0:DarkGreen} succeeded, {1:DarkBlue} skipped, {2:Red} failed.\n",
                                  succeeded, skipped, failed,
                                  cancellationToken.IsCancellationRequested ? " canceled!" : " successful!");
                              WriteLine("Time elapsed {0}", stopwatch.Elapsed);

                              SetStatusLabel(
                                  StatusType.Info,
                                  "\nBuild{3}! {0} succeeded, {1} skipped, {2} failed.",
                                  succeeded, skipped, failed,
                                  cancellationToken.IsCancellationRequested ? " canceled" : " successful");
                          }
                        , _cancellationTokenSource.Token)
                      .ConfigureAwait(false);

            SetProgressbarValue(false);
            splitContainer1.Panel1.InvokeIfRequired(t => t.Enabled = true);

            menuStrip1.InvokeIfRequired(
                m =>
                {
                    ForAll(
                        i => i.Enabled = true,
                        buildToolStripMenuItem1, cleanToolStripMenuItem,
                        rebuildToolStripMenuItem);
                    cancelBuildToolStripMenuItem.Visible = false;
                });

            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;

            Interlocked.Exchange(ref _build, 0);
        }
    }
}