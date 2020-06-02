using System.Collections.Generic;

namespace Exomia.Framework.ContentManager.IO
{
    sealed class ExporterContext
    {
        private readonly List<(string, object?[])> _messages;
        
        internal IReadOnlyList<(string text, object?[] args)> Messages
        {
            get { return _messages; }
        }

        public string ItemName    { get; }
        
        public string VirtualPath { get; }

        public string OutputFolder { get; }

        public ExporterContext(string itemName, string virtualPath, string outputFolder)
        {
            ItemName    = itemName;
            VirtualPath = virtualPath;
            OutputFolder = outputFolder;
            
            _messages   = new List<(string, object?[])>();
        }

        public void AddMessage(string text, params object?[] args)
        {
            _messages.Add((text, args));
        }
    }
}
