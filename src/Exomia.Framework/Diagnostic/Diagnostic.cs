#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Management;

namespace Exomia.Framework.Diagnostic
{
    /// <summary>
    ///     A diagnostic.
    /// </summary>
    static class Diagnostic
    {
        /// <summary>
        ///     Gets board information.
        /// </summary>
        /// <param name="outS"> [out] The out s. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        internal static bool GetBoardInformation(out string outS)
        {
            return ReadHwClassInformation("Win32_BaseBoard", out outS);
        }

        /// <summary>
        ///     Gets board property.
        /// </summary>
        /// <param name="propertyName"> Name of the property. </param>
        /// <param name="outS">         [out] The out s. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        internal static bool GetBoardProperty(string propertyName, out string outS)
        {
            return ReadHwClassProperty("Win32_BaseBoard", propertyName, out outS);
        }

        /// <summary>
        ///     Gets CPU information.
        /// </summary>
        /// <param name="outS"> [out] The out s. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        internal static bool GetCpuInformation(out string outS)
        {
            return ReadHwClassInformation("Win32_Processor", out outS);
        }

        /// <summary>
        ///     Gets CPU property.
        /// </summary>
        /// <param name="propertyName"> Name of the property. </param>
        /// <param name="outS">         [out] The out s. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        internal static bool GetCpuProperty(string propertyName, out string outS)
        {
            return ReadHwClassProperty("Win32_Processor", propertyName, out outS);
        }

        /// <summary>
        ///     Gets GPU information.
        /// </summary>
        /// <param name="outS"> [out] The out s. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        internal static bool GetGpuInformation(out string outS)
        {
            return ReadHwClassInformation("Win32_VideoController", out outS);
        }

        /// <summary>
        ///     Gets GPU property.
        /// </summary>
        /// <param name="propertyName"> Name of the property. </param>
        /// <param name="outS">         [out] The out s. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        internal static bool GetGpuProperty(string propertyName, out string outS)
        {
            return ReadHwClassProperty("Win32_VideoController", propertyName, out outS);
        }

        /// <summary>
        ///     Reads hardware class information.
        /// </summary>
        /// <param name="hwClass"> The hardware class. </param>
        /// <param name="outS">    [out] The out s. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        internal static bool ReadHwClassInformation(string hwClass, out string outS)
        {
            outS = string.Empty;
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher($"select * from {hwClass}"))
            {
                ManagementObjectCollection moc = searcher.Get();
                if (moc.Count <= 0) { return false; }
                foreach (ManagementBaseObject share in moc)
                {
                    outS +=
                        $"[{share.ToString().Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1]}]{Environment.NewLine}";
                    foreach (PropertyData property in share.Properties)
                    {
                        outS += $"\t{property.Name}={property.Value ?? string.Empty}{Environment.NewLine}";
                    }
                    outS += $"{Environment.NewLine}{Environment.NewLine}";
                }
            }
            return true;
        }

        /// <summary>
        ///     Reads hardware class property.
        /// </summary>
        /// <param name="hwClass">      The hardware class. </param>
        /// <param name="propertyName"> Name of the property. </param>
        /// <param name="outS">         [out] The out s. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        internal static bool ReadHwClassProperty(string hwClass, string propertyName, out string outS)
        {
            outS = string.Empty;
            using (ManagementObjectSearcher searcher =
                new ManagementObjectSearcher($"select {propertyName} from {hwClass}"))
            {
                ManagementObjectCollection moc = searcher.Get();
                if (moc.Count <= 0) { return false; }
                foreach (ManagementBaseObject share in moc)
                {
                    outS = share[propertyName].ToString();
                    if (!string.IsNullOrEmpty(outS)) { return true; }
                }
            }
            return false;
        }
    }
}