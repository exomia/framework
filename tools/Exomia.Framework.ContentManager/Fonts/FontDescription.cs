#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Exomia.Framework.ContentManager.Annotations;

namespace Exomia.Framework.ContentManager.Fonts
{
    [Serializable]
    class FontDescription : INotifyPropertyChanged
    {
        private string _name = "arial";
        private string _chars = "32-126,128,130-140,142,145-156,158-255";
        private int _size = 12;
        private bool _aa = true;
        private bool _isBold = false;
        private bool _isItalic = false;

        [Description("The font name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        [Description("Specify the characters to use.\ne.g. 32-126,128,130-140,142,145-156,158-255")]
        public string Chars
        {
            get { return _chars; }
            set { _chars = value; OnPropertyChanged(); }
        }

        [Description("The size of the font in pixel.")]
        public int Size
        {
            get { return _size; }
            set { _size = value; OnPropertyChanged(); }
        }

        [Description("Turn on/off antialiasing.")]
        // ReSharper disable once InconsistentNaming
        public bool AA
        {
            get { return _aa; }
            set { _aa = value; OnPropertyChanged(); }
        }

        [Description("Set if the font should be bold.")]
        public bool IsBold
        {
            get { return _isBold; }
            set { _isBold = value; OnPropertyChanged(); }
        }

        [Description("Turn on/offif the font should be italic")]
        public bool IsItalic
        {
            get { return _isItalic; }
            set { _isItalic = value; OnPropertyChanged(); }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Name} ({Size}px)";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}