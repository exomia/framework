#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Exomia.Framework.ContentManager.IO;
using Exomia.Framework.ContentManager.PropertyGridItems;

namespace Exomia.Framework.ContentManager.Converters;

/// <summary>
///     A converter. This class cannot be inherited.
/// </summary>
public sealed class ItemExporterImporterConverter : TypeConverter
{
    /// <inheritdoc />
    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
    {
        return true;
    }

    /// <inheritdoc />
    public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
    {
        return true;
    }

    /// <inheritdoc />
    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
        if (context.Instance is ItemPropertyGridItem item)
        {
            if (context.PropertyDescriptor.PropertyType == typeof(IExporter))
            {
                return new StandardValuesCollection(item.Exporters);
            }

            if (context.PropertyDescriptor.PropertyType == typeof(IImporter))
            {
                return new StandardValuesCollection(item.Importers);
            }
        }

        throw new InvalidCastException(nameof(context.Instance));
    }

    /// <inheritdoc />
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        return destinationType == typeof(IImporter) ||
            destinationType    == typeof(IExporter) ||
            base.CanConvertTo(context, destinationType);
    }

    /// <inheritdoc />
    public override object ConvertTo(ITypeDescriptorContext context,
                                     CultureInfo            culture,
                                     object                 value,
                                     Type                   destinationType)
    {
        return value switch
        {
            IImporter _ => value.GetType().GetCustomAttribute<ImporterAttribute>().Name,
            IExporter _ => value.GetType().GetCustomAttribute<ExporterAttribute>().Name,
            _           => base.ConvertTo(context, culture, value, destinationType)
        };
    }

    /// <inheritdoc />
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string) ||
            base.CanConvertFrom(context, sourceType);
    }

    /// <inheritdoc />
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string s)
        {
            if (context.Instance is ItemPropertyGridItem item)
            {
                if (context.PropertyDescriptor.PropertyType == typeof(IImporter))
                {
                    return item.Importers.First(
                        e => e.GetType().GetCustomAttribute<ImporterAttribute>().Name.Equals(s));
                }

                if (context.PropertyDescriptor.PropertyType == typeof(IExporter))
                {
                    return item.Exporters.First(
                        e => e.GetType().GetCustomAttribute<ExporterAttribute>().Name.Equals(s));
                }
            }
        }
        return base.ConvertFrom(context, culture, value);
    }
}