#region MIT License

// Copyright (c) 2018 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

using System;
using System.Management;

namespace Exomia.Framework.Diagnostic
{
    static class Diagnostic
    {
        internal static bool GetCpuInformation(out string outS)
        {
            return ReadHwClassInformation("Win32_Processor", out outS);
        }

        internal static bool GetGpuInformation(out string outS)
        {
            return ReadHwClassInformation("Win32_VideoController", out outS);
        }

        internal static bool GetBoardInformation(out string outS)
        {
            return ReadHwClassInformation("Win32_BaseBoard", out outS);
        }

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

        internal static bool GetCpuProperty(string propertyeName, out string outS)
        {
            return ReadHwClassProperty("Win32_Processor", propertyeName, out outS);
        }

        internal static bool GetGpuProperty(string propertyeName, out string outS)
        {
            return ReadHwClassProperty("Win32_VideoController", propertyeName, out outS);
        }

        internal static bool GetBoardProperty(string propertyeName, out string outS)
        {
            return ReadHwClassProperty("Win32_BaseBoard", propertyeName, out outS);
        }

        internal static bool ReadHwClassProperty(string hwClass, string propertyeName, out string outS)
        {
            outS = string.Empty;
            using (ManagementObjectSearcher searcher =
                new ManagementObjectSearcher($"select {propertyeName} from {hwClass}"))
            {
                ManagementObjectCollection moc = searcher.Get();
                if (moc.Count <= 0) { return false; }
                foreach (ManagementBaseObject share in moc)
                {
                    outS = share[propertyeName].ToString();
                    if (!string.IsNullOrEmpty(outS)) { return true; }
                }
            }
            return false;
        }
    }
}