using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using Exomia.Framework.ContentManager.Extensions;
using Exomia.Framework.ContentManager.IO;
using Exomia.Framework.ContentManager.PropertyGridItems;

namespace Exomia.Framework.ContentManager
{
    partial class MainForm
    {
        private void Build()
        {
            SetProgressbarValue(true);
            SetStatusLabel(
                StatusType.Info,
                "Build started {0}",
                DateTime.Now.ToString("dd/MMM/yyyy HH:mm:ss \"UTC\"z", CultureInfo.InvariantCulture));
            Clear();
            WriteLine(
                "Build started {0}",
                DateTime.Now.ToString("dd/MMM/yyyy HH:mm:ss \"UTC\"z", CultureInfo.InvariantCulture));

            Stopwatch stopwatch = Stopwatch.StartNew();

            int succeeded = 0;
            int skipped   = 0;
            int failed    = 0;

            treeView1.InvokeIfRequired(
                x =>
                {
                    var rootNode = x.Nodes[ROOT_KEY_PREFIX];

                    void ForTreeNode(TreeNode node, ContentPropertyGridItem contentPropertyGridItem)
                    {
                        foreach (TreeNode treeNode in node.Nodes)
                        {
                            ForTreeNode(treeNode, contentPropertyGridItem);
                        }

                        if (node.Tag is ItemPropertyGridItem gridItem)
                        {
                            if (gridItem.BuildAction == BuildAction.Ignore)
                            {
                                WriteLine(
                                    "Item {0} is ignored during build! skipping...!",
                                    Path.Combine(gridItem.VirtualPath, gridItem.Name));
                                Interlocked.Increment(ref skipped);
                                return;
                            }
                            
                            if (gridItem.Importer == null)
                            {
                                WriteLine(
                                    "skipping item {0} no importer set!",
                                    Path.Combine(gridItem.VirtualPath, gridItem.Name));
                                Interlocked.Increment(ref skipped);
                                return;
                            }

                            if (gridItem.Exporter == null)
                            {
                                WriteLine(
                                    "skipping item {0} no exporter set!",
                                    Path.Combine(gridItem.VirtualPath, gridItem.Name));
                                Interlocked.Increment(ref skipped);
                                return;
                            }

                            void WriteLineMessages(IEnumerable<(string, object?[])> messages)
                            {
                                foreach (var (text, args) in messages)
                                {
                                    WriteLine($"\t> {text}", args);
                                }
                            }

                            WriteLine("Import item {0}...", Path.Combine(gridItem.VirtualPath, gridItem.Name));
                            ImporterContext importerContext = new ImporterContext(gridItem.Name, gridItem.VirtualPath);

                            Stream stream;
                            if (gridItem is FontPropertyGridItem fontItem)
                            {
                                fontItem.Serialize(stream = new MemoryStream());
                            }
                            else
                            {
                                stream = new MemoryStream();
                            }
                            
                            object? obj;
                            using (stream)
                            {
                                obj = gridItem.Importer.Import(stream, importerContext);
                            }
                            if (obj == null)
                            {
                                WriteLine("\tfailed with messages:");
                                WriteLineMessages(importerContext.Messages);
                                Interlocked.Increment(ref failed);
                                return;
                            }
                            WriteLineMessages(importerContext.Messages);

                            WriteLine("Export item {0}...", Path.Combine(gridItem.VirtualPath, gridItem.Name));
                            ExporterContext exporterContext = new ExporterContext(
                                gridItem.Name,
                                gridItem.VirtualPath,
                                contentPropertyGridItem.OutputFolder ??
                                Path.GetDirectoryName(contentPropertyGridItem.ProjectLocation));

                            if (!gridItem.Exporter.Export(obj, exporterContext))
                            {
                                WriteLine("\tfailed with messages:");
                                WriteLineMessages(exporterContext.Messages);
                                Interlocked.Increment(ref failed);
                                return;
                            }
                            WriteLineMessages(exporterContext.Messages);
                            Interlocked.Increment(ref succeeded);
                        }
                    }

                    ForTreeNode(rootNode, (ContentPropertyGridItem)rootNode.Tag);
                });

            stopwatch.Stop();
            WriteLine(
                "\nBuild {0:DarkGreen} succeeded, {1:DarkBlue} skipped, {2:Red} failed.\n",
                succeeded, skipped, failed);
            WriteLine("Time elapsed {0}", stopwatch.Elapsed);

            SetStatusLabel(
                StatusType.Info,
                "\nBuild {0} succeeded, {1} skipped, {2} failed.\n",
                succeeded, skipped, failed);
            SetProgressbarValue(false);
        }
    }
}