#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

// ReSharper disable InconsistentNaming
namespace Exomia.Framework.Input
{
    /// <summary>
    ///     A keys.
    /// </summary>
    public static class Key
    {
        /// <summary>
        ///     The key code bit mask.
        /// </summary>
        public const int KEY_CODE = 0x0000FFFF;

        /// <summary>
        ///     The modifiers bit mask.
        /// </summary>
        public const int MODIFIERS = unchecked((int)0xFFFF0000);

        /// <summary>
        ///     The none.
        /// </summary>
        public const int None = 0x00;

        /// <summary>
        ///     The cancel.
        /// </summary>
        public const int Cancel = 0x03;

        /// <summary>
        ///     The back.
        /// </summary>
        public const int Back = 0x08;

        /// <summary>
        ///     The tab.
        /// </summary>
        public const int Tab = 0x09;

        /// <summary>
        ///     The line feed.
        /// </summary>
        public const int LineFeed = 0x0A;

        /// <summary>
        ///     The clear.
        /// </summary>
        public const int Clear = 0x0C;

        /// <summary>
        ///     The return.
        /// </summary>
        public const int Return = 0x0D;

        /// <summary>
        ///     The enter.
        /// </summary>
        public const int Enter = Return;

        /// <summary>
        ///     The shift key.
        /// </summary>
        public const int ShiftKey = 0x10;

        /// <summary>
        ///     The control key.
        /// </summary>
        public const int ControlKey = 0x11;

        /// <summary>
        ///     The menu (alt) key.
        /// </summary>
        public const int Menu = 0x12;

        /// <summary>
        ///     The shift modifier.
        /// </summary>
        public const int Shift = 0x00010000;

        /// <summary>
        ///     The control modifier.
        /// </summary>
        public const int Control = 0x00020000;

        /// <summary>
        ///     The alternate modifier.
        /// </summary>
        public const int Alt = 0x00040000;

        /// <summary>
        ///     The pause.
        /// </summary>
        public const int Pause = 0x13;

        /// <summary>
        ///     The capital.
        /// </summary>
        public const int Capital = 0x14;

        /// <summary>
        ///     The capabilities lock.
        /// </summary>
        public const int CapsLock = 0x14;

        /// <summary>
        ///     The kana mode.
        /// </summary>
        public const int KanaMode = 0x15;

        /// <summary>
        ///     The hanguel mode.
        /// </summary>
        public const int HanguelMode = 0x15;

        /// <summary>
        ///     The hangul mode.
        /// </summary>
        public const int HangulMode = 0x15;

        /// <summary>
        ///     The junja mode.
        /// </summary>
        public const int JunjaMode = 0x17;

        /// <summary>
        ///     The final mode.
        /// </summary>
        public const int FinalMode = 0x18;

        /// <summary>
        ///     The hanja mode.
        /// </summary>
        public const int HanjaMode = 0x19;

        /// <summary>
        ///     The kanji mode.
        /// </summary>
        public const int KanjiMode = 0x19;

        /// <summary>
        ///     The escape.
        /// </summary>
        public const int Escape = 0x1B;

        /// <summary>
        ///     The ime convert.
        /// </summary>
        public const int IMEConvert = 0x1C;

        /// <summary>
        ///     The ime nonconvert.
        /// </summary>
        public const int IMENonconvert = 0x1D;

        /// <summary>
        ///     The ime accept.
        /// </summary>
        public const int IMEAccept = 0x1E;

        /// <summary>
        ///     The ime aceept.
        /// </summary>
        public const int IMEAceept = IMEAccept;

        /// <summary>
        ///     The ime mode change.
        /// </summary>
        public const int IMEModeChange = 0x1F;

        /// <summary>
        ///     The space.
        /// </summary>
        public const int Space = 0x20;

        /// <summary>
        ///     The prior.
        /// </summary>
        public const int Prior = 0x21;

        /// <summary>
        ///     The page up.
        /// </summary>
        public const int PageUp = Prior;

        /// <summary>
        ///     The next.
        /// </summary>
        public const int Next = 0x22;

        /// <summary>
        ///     The page down.
        /// </summary>
        public const int PageDown = Next;

        /// <summary>
        ///     The end.
        /// </summary>
        public const int End = 0x23;

        /// <summary>
        ///     The home.
        /// </summary>
        public const int Home = 0x24;

        /// <summary>
        ///     The left.
        /// </summary>
        public const int Left = 0x25;

        /// <summary>
        ///     The up.
        /// </summary>
        public const int Up = 0x26;

        /// <summary>
        ///     The right.
        /// </summary>
        public const int Right = 0x27;

        /// <summary>
        ///     The down.
        /// </summary>
        public const int Down = 0x28;

        /// <summary>
        ///     The select.
        /// </summary>
        public const int Select = 0x29;

        /// <summary>
        ///     The print.
        /// </summary>
        public const int Print = 0x2A;

        /// <summary>
        ///     The execute.
        /// </summary>
        public const int Execute = 0x2B;

        /// <summary>
        ///     The snapshot.
        /// </summary>
        public const int Snapshot = 0x2C;

        /// <summary>
        ///     The print screen.
        /// </summary>
        public const int PrintScreen = Snapshot;

        /// <summary>
        ///     The insert.
        /// </summary>
        public const int Insert = 0x2D;

        /// <summary>
        ///     The delete.
        /// </summary>
        public const int Delete = 0x2E;

        /// <summary>
        ///     The help.
        /// </summary>
        public const int Help = 0x2F;

        /// <summary>
        ///     0.
        /// </summary>
        public const int D0 = 0x30;

        /// <summary>
        ///     1.
        /// </summary>
        public const int D1 = 0x31;

        /// <summary>
        ///     2.
        /// </summary>
        public const int D2 = 0x32;

        /// <summary>
        ///     3.
        /// </summary>
        public const int D3 = 0x33;

        /// <summary>
        ///     4.
        /// </summary>
        public const int D4 = 0x34;

        /// <summary>
        ///     5.
        /// </summary>
        public const int D5 = 0x35;

        /// <summary>
        ///     6.
        /// </summary>
        public const int D6 = 0x36;

        /// <summary>
        ///     7.
        /// </summary>
        public const int D7 = 0x37;

        /// <summary>
        ///     8.
        /// </summary>
        public const int D8 = 0x38;

        /// <summary>
        ///     9.
        /// </summary>
        public const int D9 = 0x39;

        /// <summary>
        ///     The const int to process.
        /// </summary>
        public const int A = 0x41;

        /// <summary>
        ///     The const int to process.
        /// </summary>
        public const int B = 0x42;

        /// <summary>
        ///     The const int to process.
        /// </summary>
        public const int C = 0x43;

        /// <summary>
        ///     The const int to process.
        /// </summary>
        public const int D = 0x44;

        /// <summary>
        ///     The const int to process.
        /// </summary>
        public const int E = 0x45;

        /// <summary>
        ///     The const int to process.
        /// </summary>
        public const int F = 0x46;

        /// <summary>
        ///     The const int to process.
        /// </summary>
        public const int G = 0x47;

        /// <summary>
        ///     The const int to process.
        /// </summary>
        public const int H = 0x48;

        /// <summary>
        ///     Zero-based index of the.
        /// </summary>
        public const int I = 0x49;

        /// <summary>
        ///     The const int to process.
        /// </summary>
        public const int J = 0x4A;

        /// <summary>
        ///     The const int to process.
        /// </summary>
        public const int K = 0x4B;

        /// <summary>
        ///     The const int to process.
        /// </summary>
        public const int L = 0x4C;

        /// <summary>
        ///     The const int to process.
        /// </summary>
        public const int M = 0x4D;

        /// <summary>
        ///     The const int to process.
        /// </summary>
        public const int N = 0x4E;

        /// <summary>
        ///     The const int to process.
        /// </summary>
        public const int O = 0x4F;

        /// <summary>
        ///     The const int to process.
        /// </summary>
        public const int P = 0x50;

        /// <summary>
        ///     The const int to process.
        /// </summary>
        public const int Q = 0x51;

        /// <summary>
        ///     The const int to process.
        /// </summary>
        public const int R = 0x52;

        /// <summary>
        ///     The const int to process.
        /// </summary>
        public const int S = 0x53;

        /// <summary>
        ///     The const int to process.
        /// </summary>
        public const int T = 0x54;

        /// <summary>
        ///     The const int to process.
        /// </summary>
        public const int U = 0x55;

        /// <summary>
        ///     The const int to process.
        /// </summary>
        public const int V = 0x56;

        /// <summary>
        ///     The const int to process.
        /// </summary>
        public const int W = 0x57;

        /// <summary>
        ///     The const int to process.
        /// </summary>
        public const int X = 0x58;

        /// <summary>
        ///     The const int to process.
        /// </summary>
        public const int Y = 0x59;

        /// <summary>
        ///     The const int to process.
        /// </summary>
        public const int Z = 0x5A;

        /// <summary>
        ///     The window.
        /// </summary>
        public const int LWin = 0x5B;

        /// <summary>
        ///     The window.
        /// </summary>
        public const int RWin = 0x5C;

        /// <summary>
        ///     The apps.
        /// </summary>
        public const int Apps = 0x5D;

        /// <summary>
        ///     The sleep.
        /// </summary>
        public const int Sleep = 0x5F;

        /// <summary>
        ///     Number of pad 0s.
        /// </summary>
        public const int NumPad0 = 0x60;

        /// <summary>
        ///     Number of pad 1s.
        /// </summary>
        public const int NumPad1 = 0x61;

        /// <summary>
        ///     Number of pad 2s.
        /// </summary>
        public const int NumPad2 = 0x62;

        /// <summary>
        ///     Number of pad 3s.
        /// </summary>
        public const int NumPad3 = 0x63;

        /// <summary>
        ///     Number of pad 4s.
        /// </summary>
        public const int NumPad4 = 0x64;

        /// <summary>
        ///     Number of pad 5s.
        /// </summary>
        public const int NumPad5 = 0x65;

        /// <summary>
        ///     Number of pad 6s.
        /// </summary>
        public const int NumPad6 = 0x66;

        /// <summary>
        ///     Number of pad 7s.
        /// </summary>
        public const int NumPad7 = 0x67;

        /// <summary>
        ///     Number of pad 8s.
        /// </summary>
        public const int NumPad8 = 0x68;

        /// <summary>
        ///     Number of pad 9s.
        /// </summary>
        public const int NumPad9 = 0x69;

        /// <summary>
        ///     The multiply.
        /// </summary>
        public const int Multiply = 0x6A;

        /// <summary>
        ///     The add.
        /// </summary>
        public const int Add = 0x6B;

        /// <summary>
        ///     The separator.
        /// </summary>
        public const int Separator = 0x6C;

        /// <summary>
        ///     The subtract.
        /// </summary>
        public const int Subtract = 0x6D;

        /// <summary>
        ///     The decimal.
        /// </summary>
        public const int Decimal = 0x6E;

        /// <summary>
        ///     The divide.
        /// </summary>
        public const int Divide = 0x6F;

        /// <summary>
        ///     The f1 key.
        /// </summary>
        public const int F1 = 0x70;

        /// <summary>
        ///     The f2 key.
        /// </summary>
        public const int F2 = 0x71;

        /// <summary>
        ///     The f3 key.
        /// </summary>
        public const int F3 = 0x72;

        /// <summary>
        ///     The f4 key.
        /// </summary>
        public const int F4 = 0x73;

        /// <summary>
        ///     The f5 key.
        /// </summary>
        public const int F5 = 0x74;

        /// <summary>
        ///     The f6 key.
        /// </summary>
        public const int F6 = 0x75;

        /// <summary>
        ///     The f7 key.
        /// </summary>
        public const int F7 = 0x76;

        /// <summary>
        ///     The f8 key.
        /// </summary>
        public const int F8 = 0x77;

        /// <summary>
        ///     The f9 key.
        /// </summary>
        public const int F9 = 0x78;

        /// <summary>
        ///     The f10 key.
        /// </summary>
        public const int F10 = 0x79;

        /// <summary>
        ///     The f11 key.
        /// </summary>
        public const int F11 = 0x7A;

        /// <summary>
        ///     The f12 key.
        /// </summary>
        public const int F12 = 0x7B;

        /// <summary>
        ///     The f13 key.
        /// </summary>
        public const int F13 = 0x7C;

        /// <summary>
        ///     The f14 key.
        /// </summary>
        public const int F14 = 0x7D;

        /// <summary>
        ///     The f15 key.
        /// </summary>
        public const int F15 = 0x7E;

        /// <summary>
        ///     The f16 key.
        /// </summary>
        public const int F16 = 0x7F;

        /// <summary>
        ///     The f17 key.
        /// </summary>
        public const int F17 = 0x80;

        /// <summary>
        ///     The f18 key.
        /// </summary>
        public const int F18 = 0x81;

        /// <summary>
        ///     The f19 key.
        /// </summary>
        public const int F19 = 0x82;

        /// <summary>
        ///     The f20 key.
        /// </summary>
        public const int F20 = 0x83;

        /// <summary>
        ///     The f21 key.
        /// </summary>
        public const int F21 = 0x84;

        /// <summary>
        ///     The the f22 key.
        /// </summary>
        public const int F22 = 0x85;

        /// <summary>
        ///     The f23 ke.
        /// </summary>
        public const int F23 = 0x86;

        /// <summary>
        ///     The f24 key.
        /// </summary>
        public const int F24 = 0x87;

        /// <summary>
        ///     Number of locks.
        /// </summary>
        public const int NumLock = 0x90;

        /// <summary>
        ///     The scroll.
        /// </summary>
        public const int Scroll = 0x91;

        /// <summary>
        ///     The left shift key.
        /// </summary>
        public const int LShift = 0xA0;

        /// <summary>
        ///     The right shift key.
        /// </summary>
        public const int RShift = 0xA1;

        /// <summary>
        ///     The left control key.
        /// </summary>
        public const int LControl = 0xA2;

        /// <summary>
        ///     The right control key.
        /// </summary>
        public const int RControl = 0xA3;

        /// <summary>
        ///     The left menu (alt) key.
        /// </summary>
        public const int LMenu = 0xA4;

        /// <summary>
        ///     The right menu (alt) key.
        /// </summary>
        public const int RMenu = 0xA5;

        /// <summary>
        ///     The browser back.
        /// </summary>
        public const int BrowserBack = 0xA6;

        /// <summary>
        ///     The browser forward.
        /// </summary>
        public const int BrowserForward = 0xA7;

        /// <summary>
        ///     The browser refresh.
        /// </summary>
        public const int BrowserRefresh = 0xA8;

        /// <summary>
        ///     The browser stop.
        /// </summary>
        public const int BrowserStop = 0xA9;

        /// <summary>
        ///     The browser search.
        /// </summary>
        public const int BrowserSearch = 0xAA;

        /// <summary>
        ///     The browser favorites.
        /// </summary>
        public const int BrowserFavorites = 0xAB;

        /// <summary>
        ///     The browser home.
        /// </summary>
        public const int BrowserHome = 0xAC;

        /// <summary>
        ///     The volume mute.
        /// </summary>
        public const int VolumeMute = 0xAD;

        /// <summary>
        ///     The volume down.
        /// </summary>
        public const int VolumeDown = 0xAE;

        /// <summary>
        ///     The volume up.
        /// </summary>
        public const int VolumeUp = 0xAF;

        /// <summary>
        ///     The media next track.
        /// </summary>
        public const int MediaNextTrack = 0xB0;

        /// <summary>
        ///     The media previous track.
        /// </summary>
        public const int MediaPreviousTrack = 0xB1;

        /// <summary>
        ///     The media stop.
        /// </summary>
        public const int MediaStop = 0xB2;

        /// <summary>
        ///     The media play pause.
        /// </summary>
        public const int MediaPlayPause = 0xB3;

        /// <summary>
        ///     The launch mail.
        /// </summary>
        public const int LaunchMail = 0xB4;

        /// <summary>
        ///     The select media.
        /// </summary>
        public const int SelectMedia = 0xB5;

        /// <summary>
        ///     The first launch application.
        /// </summary>
        public const int LaunchApplication1 = 0xB6;

        /// <summary>
        ///     The second launch application.
        /// </summary>
        public const int LaunchApplication2 = 0xB7;

        /// <summary>
        ///     The OEM semicolon.
        /// </summary>
        public const int OemSemicolon = 0xBA;

        /// <summary>
        ///     The first OEM.
        /// </summary>
        public const int Oem1 = OemSemicolon;

        /// <summary>
        ///     The oemplus.
        /// </summary>
        public const int Oemplus = 0xBB;

        /// <summary>
        ///     The oemcomma.
        /// </summary>
        public const int Oemcomma = 0xBC;

        /// <summary>
        ///     The OEM minus.
        /// </summary>
        public const int OemMinus = 0xBD;

        /// <summary>
        ///     The OEM period.
        /// </summary>
        public const int OemPeriod = 0xBE;

        /// <summary>
        ///     The OEM question.
        /// </summary>
        public const int OemQuestion = 0xBF;

        /// <summary>
        ///     The second OEM.
        /// </summary>
        public const int Oem2 = OemQuestion;

        /// <summary>
        ///     The oemtilde.
        /// </summary>
        public const int Oemtilde = 0xC0;

        /// <summary>
        ///     The third OEM.
        /// </summary>
        public const int Oem3 = Oemtilde;

        /// <summary>
        ///     The OEM open brackets.
        /// </summary>
        public const int OemOpenBrackets = 0xDB;

        /// <summary>
        ///     The fourth OEM.
        /// </summary>
        public const int Oem4 = OemOpenBrackets;

        /// <summary>
        ///     The OEM pipe.
        /// </summary>
        public const int OemPipe = 0xDC;

        /// <summary>
        ///     The fifth OEM.
        /// </summary>
        public const int Oem5 = OemPipe;

        /// <summary>
        ///     The OEM close brackets.
        /// </summary>
        public const int OemCloseBrackets = 0xDD;

        /// <summary>
        ///     The OEM 6.
        /// </summary>
        public const int Oem6 = OemCloseBrackets;

        /// <summary>
        ///     The OEM quotes.
        /// </summary>
        public const int OemQuotes = 0xDE;

        /// <summary>
        ///     The OEM 7.
        /// </summary>
        public const int Oem7 = OemQuotes;

        /// <summary>
        ///     The OEM 8.
        /// </summary>
        public const int Oem8 = 0xDF;

        /// <summary>
        ///     The OEM backslash.
        /// </summary>
        public const int OemBackslash = 0xE2;

        /// <summary>
        ///     The second OEM 10.
        /// </summary>
        public const int Oem102 = OemBackslash;

        /// <summary>
        ///     The process key.
        /// </summary>
        public const int Process = 0xE5;

        /// <summary>
        ///     The packet.
        /// </summary>
        public const int Packet = 0xE7;

        /// <summary>
        ///     The attn.
        /// </summary>
        public const int Attn = 0xF6;

        /// <summary>
        ///     The crsel.
        /// </summary>
        public const int Crsel = 0xF7;

        /// <summary>
        ///     The exsel.
        /// </summary>
        public const int Exsel = 0xF8;

        /// <summary>
        ///     The erase EOF.
        /// </summary>
        public const int EraseEof = 0xF9;

        /// <summary>
        ///     The play.
        /// </summary>
        public const int Play = 0xFA;

        /// <summary>
        ///     The zoom.
        /// </summary>
        public const int Zoom = 0xFB;

        /// <summary>
        ///     Name of the no.
        /// </summary>
        public const int NoName = 0xFC;

        /// <summary>
        ///     The first pa.
        /// </summary>
        public const int Pa1 = 0xFD;

        /// <summary>
        ///     The OEM clear.
        /// </summary>
        public const int OemClear = 0xFE;
    }
}