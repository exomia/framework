#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Exomia.Framework.ContentManager.Extensions;

namespace Exomia.Framework.ContentManager;

/// <summary>
///     The application's main form.
/// </summary>
public partial class MainForm : Form
{
    private ProjectFile? _projectFile;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MainForm" /> class.
    /// </summary>
    public MainForm()
    {
        InitializeComponent();
        SetProgressbarValue(false);

        treeView1.TreeViewNodeSorter = new NodeSorter();
    }

    /// <summary>
    ///     Sets status label.
    /// </summary>
    /// <param name="statusType"> Type of the status. </param>
    /// <param name="text">       The text. </param>
    /// <param name="args">       A variable-length parameters list containing arguments. </param>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
    public void SetStatusLabel(StatusType statusType, string text, params object?[] args)
    {
        statusStrip1.InvokeIfRequired(
            x =>
            {
                toolStripStatusLabel1.Image = statusType switch
                {
                    StatusType.Warning => Properties.Resources
                                                    .StatusWarning_16x,
                    StatusType.Error => Properties.Resources
                                                  .StatusCriticalError_16x,
                    StatusType.Info => Properties.Resources
                                                 .StatusInformation_16x,
                    _ => throw new ArgumentOutOfRangeException(nameof(statusType), statusType, null)
                };
                toolStripStatusLabel1.Text = string.Format(text, args);
            });
    }

    /// <summary>
    ///     Sets progressbar value.
    /// </summary>
    /// <param name="state"> true to set progressbar to running state; false otherwise. </param>
    public void SetProgressbarValue(bool state)
    {
        statusStrip1.InvokeIfRequired(
            x =>
            {
                toolStripProgressBar1.Style = state
                    ? ProgressBarStyle.Marquee
                    : ProgressBarStyle.Continuous;
                toolStripProgressBar1.Value = 0;
            });
    }

    private void Clear()
    {
        richTextBox1.InvokeIfRequired(
            x => { x.Clear(); });
    }

    private void WriteLine(string text, params object?[] args)
    {
        richTextBox1.InvokeIfRequired(
            x =>
            {
                MatchCollection? matches = Regex.Matches(text, "\\{([0-9]+)(?:\\:([A-Za-z]+))?\\}");

                if (matches.Count <= 0)
                {
                    x.AppendText($"{string.Format(text, args)}{Environment.NewLine}");
                    return;
                }

                using Font boldFont = new Font(x.Font, FontStyle.Bold);

                int current = 0;
                foreach (Match match in matches)
                {
                    x.AppendText(text.Substring(current, match.Index - current));
                    current = match.Index + match.Length;

                    x.SelectionStart  = x.TextLength;
                    x.SelectionLength = 0;
                    x.SelectionFont   = boldFont;

                    object? value = args[int.Parse(match.Groups[1].Value)];

                    if (match.Groups[2].Success)
                    {
                        x.SelectionColor = Color.FromName(match.Groups[2].Value);
                    }
                    else if (value is string)
                    {
                        x.SelectionColor = Color.DarkOrange;
                    }
                    else if (IsNumber(value))
                    {
                        x.SelectionColor = Color.DarkBlue;
                    }
                    else
                    {
                        x.SelectionColor = Color.DarkGreen;
                    }

                    x.AppendText(value?.ToString());
                    x.SelectionFont  = x.Font;
                    x.SelectionColor = x.ForeColor;
                }

                x.AppendText($"{text.Substring(current, text.Length - current)}{Environment.NewLine}");
                x.ScrollToCaret();
            });
    }

    private void WriteLineMessages(IEnumerable<(string, object?[])> messages)
    {
        foreach (var (text, args) in messages)
        {
            WriteLine($"\t> {text}", args);
        }
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        Save();
        _projectFile = null;
    }

    private static bool IsNumber(object? value)
    {
        return value is sbyte
            || value is byte
            || value is short
            || value is ushort
            || value is int
            || value is uint
            || value is long
            || value is ulong
            || value is float
            || value is double
            || value is decimal;
    }

    private static void ForAll<T>(Action<T> action, params T[] items)
    {
        foreach (T item in items)
        {
            action(item);
        }
    }

    private class NodeSorter : IComparer
    {
        // Compare the length of the strings, or the strings
        // themselves, if they are the same length.
        public int Compare(object? x, object? y)
        {
            if (x is TreeNode tx && y is TreeNode ty)
            {
                if (tx.Name.StartsWith(FOLDER_KEY_PREFIX) && !ty.Name.StartsWith(FOLDER_KEY_PREFIX))
                {
                    return -1;
                }
                if (!tx.Name.StartsWith(FOLDER_KEY_PREFIX) && ty.Name.StartsWith(FOLDER_KEY_PREFIX))
                {
                    return 1;
                }
                return string.Compare(tx.Text, ty.Text, StringComparison.InvariantCultureIgnoreCase);
            }

            return 0;
        }
    }
}