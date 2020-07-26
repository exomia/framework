#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Collections.Generic;

namespace Exomia.Framework.ContentManager.IO
{
    sealed class ExporterContext
    {
        private readonly List<(string, object?[])> _messages;

        public string ItemName { get; }

        public string VirtualPath { get; }

        public string OutputFolder { get; }

        internal IReadOnlyList<(string text, object?[] args)> Messages
        {
            get { return _messages; }
        }

        public ExporterContext(string itemName, string virtualPath, string outputFolder)
        {
            ItemName     = itemName;
            VirtualPath  = virtualPath;
            OutputFolder = outputFolder;

            _messages = new List<(string, object?[])>();
        }

        public void AddMessage(string text, params object?[] args)
        {
            _messages.Add((text, args));
        }
    }
}