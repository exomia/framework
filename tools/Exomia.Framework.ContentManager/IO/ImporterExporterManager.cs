#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Reflection;

namespace Exomia.Framework.ContentManager.IO;

/// <summary>
///     Manager for importer and exporters.
/// </summary>
static class ImporterExporterManager
{
    private static readonly IDictionary<string, List<IImporter>> s_importers;
    private static readonly IDictionary<Type, List<IExporter>>   s_exporters;

    /// <summary>
    ///     Initializes static members of the <see cref="ImporterExporterManager" /> class.
    /// </summary>
    static ImporterExporterManager()
    {
        s_importers = new Dictionary<string, List<IImporter>>();
        s_exporters = new Dictionary<Type, List<IExporter>>();

        foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (a.FullName!.StartsWith("System", StringComparison.InvariantCultureIgnoreCase)) { continue; }
            if (a.FullName!.StartsWith("ms",     StringComparison.InvariantCultureIgnoreCase)) { continue; }

            foreach (Type t in a.GetTypes())
            {
                if (((t.IsClass && !t.IsInterface) || (t.IsValueType && !t.IsEnum)) && !t.IsAbstract)
                {
                    if (typeof(IImporter).IsAssignableFrom(t))
                    {
                        ImporterAttribute importerAttribute
                            = t.GetCustomAttribute<ImporterAttribute>(false)!;
                        IImporter importer = Activator.CreateInstance(t)
                            as IImporter ?? throw new TypeLoadException(
                            $"Can't create an instance of {nameof(IImporter)} from type: {t.AssemblyQualifiedName}");
                        foreach (string extension in importerAttribute.Extensions)
                        {
                            if (!s_importers.TryGetValue(
                                    extension.StartsWith(".")
                                        ? extension
                                        : $".{extension}", out List<IImporter>? importers))
                            {
                                s_importers.Add(extension, importers = new List<IImporter>());
                            }
                            importers.Add(importer);
                        }
                    }
                    else if (typeof(IExporter).IsAssignableFrom(t))
                    {
                        IExporter exporter = Activator.CreateInstance(t)
                            as IExporter ?? throw new TypeLoadException(
                            $"Can't create an instance of {nameof(IImporter)} from type: {t.AssemblyQualifiedName}");

                        if (!s_exporters.TryGetValue(exporter.ImportType, out List<IExporter>? exporters))
                        {
                            s_exporters.Add(exporter.ImportType, exporters = new List<IExporter>());
                        }
                        exporters.Add(exporter);
                    }
                }
            }
        }
    }

    public static List<IImporter> GetImporterFor(string extension)
    {
        if (!s_importers.TryGetValue(extension, out List<IImporter>? importers))
        {
            throw new Exception($"No importer for extension '{extension}' registered!");
        }
        return importers;
    }

    public static List<IExporter> GetExportersFor(Type importedType)
    {
        if (!s_exporters.TryGetValue(importedType, out List<IExporter>? exporters))
        {
            throw new Exception($"No exporter for import type '{importedType}' registered!");
        }
        return exporters;
    }
}