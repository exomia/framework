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
    sealed class ImporterContext
    {
        private readonly List<(string, object?[])> _messages;

        internal IReadOnlyList<(string text, object?[] args)> Messages
        {
            get { return _messages; }
        }

        public string ItemName { get; }
        
        public string VirtualPath { get; }
        

        public ImporterContext(string itemName, string virtualPath)
        {
            ItemName = itemName;
            VirtualPath = virtualPath;
            _messages = new List<(string, object?[])>();
        }

        public void AddMessage(string text, params object?[] args)
        {
            _messages.Add((text, args));
        }
    }
}