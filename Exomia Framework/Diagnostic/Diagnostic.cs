using System;
using System.Management;

namespace Exomia.Framework
{
    internal static class Diagnostic
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