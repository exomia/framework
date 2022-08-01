#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Exomia.Framework.ContentManager.Attributes;

namespace Exomia.Framework.ContentManager.Converters;

/// <summary>
///     The choices string converter.
/// </summary>
public sealed class ChoicesStringConverter : StringConverter
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
        foreach (Attribute propertyDescriptorAttribute in context.PropertyDescriptor.Attributes)
        {
            if (propertyDescriptorAttribute is ChoicesAttribute choices)
            {
                return new StandardValuesCollection(new List<object>(choices.Entries));
            }
        }
        throw new ArgumentNullException(nameof(ChoicesAttribute));
    }
}