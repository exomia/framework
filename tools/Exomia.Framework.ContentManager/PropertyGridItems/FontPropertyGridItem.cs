#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Exomia.Framework.ContentManager.Fonts;
using Exomia.Framework.ContentManager.IO;
using Exomia.Framework.ContentManager.IO.Exporter;
using Exomia.Framework.ContentManager.IO.Importer;

namespace Exomia.Framework.ContentManager.PropertyGridItems
{
    /// <summary>
    ///     A font property grid item. This class cannot be inherited.
    /// </summary>
    sealed class FontPropertyGridItem : ItemPropertyGridItem, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly IFormatter               _formatter = new BinaryFormatter();

        /// <summary>
        ///     The font description.
        /// </summary>
        /// <value>
        ///     The font.
        /// </value>
        [Category("Font Settings")]
        [Description("The font description.")]
        [DisplayName("Font")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public FontDescription FontDescription { get; private set; }

        /// <inheritdoc />
        public override byte[] Data
        {
            get
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    _formatter.Serialize(ms, FontDescription);
                    return ms.ToArray();
                }
            }
            set
            {
                using (MemoryStream ms = new MemoryStream(value))
                {
                    FontDescription = (FontDescription)_formatter.Deserialize(ms);
                }
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FolderPropertyGridItem" /> class.
        /// </summary>
        /// <param name="fontDescription">     The font description. </param>
        /// <param name="nameProvider">        The name provider. </param>
        /// <param name="virtualPathProvider"> The virtual path provider. </param>
        public FontPropertyGridItem(FontDescription        fontDescription,
                                    Provider.Value<string> nameProvider,
                                    Provider.Value<string> virtualPathProvider)
            : base(
                nameProvider, virtualPathProvider,
                new IImporter[] { BMFontImporter.Default },
                new IExporter[] { SpiteFontExporter.Default })
        {
            FontDescription = fontDescription;
            fontDescription.PropertyChanged += (sender, args) =>
            {
                OnPropertyChanged(nameof(FontDescription));
            };
            Importer = BMFontImporter.Default;
            Exporter = SpiteFontExporter.Default;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}