#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.ContentManager.IO;

sealed class ImporterContext
{
    private readonly List<(string, object?[])> _messages;

    public string  FileName         { get; }
    public string  ItemName         { get; }
    public string  VirtualPath      { get; }
    public object? ImporterSettings { get; }

    internal IReadOnlyList<(string text, object?[] args)> Messages
    {
        get { return _messages; }
    }

    public ImporterContext(string fileName, string itemName, string virtualPath, object? importerSettings)
    {
        FileName         = fileName;
        ItemName         = itemName;
        VirtualPath      = virtualPath;
        ImporterSettings = importerSettings;
        _messages        = new List<(string, object?[])>();
    }

    public void AddMessage(string text, params object?[] args)
    {
        _messages.Add((text, args));
    }
}