#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Win32
{
    /// <summary>
    ///     The tme flags.
    ///     It can be a combination of the following values.
    /// </summary>
    /// <remarks>
    ///     http://msdn.microsoft.com/en-us/library/ms645604%28v=vs.85%29.aspx.
    /// </remarks>
    static class TME
    {
        /// <summary>
        ///     The caller wants to cancel a prior tracking request.
        ///     The caller should also specify the type of tracking that it wants to cancel.
        ///     For example, to cancel hover tracking, the caller must pass the TME_CANCEL and TME_HOVER flags.
        /// </summary>
        public const uint CANCEL = 0x80000000;

        /// <summary>
        ///     The caller wants hover notification. Notification is delivered as a WM_MOUSEHOVER message.
        ///     If the caller requests hover tracking while hover tracking is already active, the hover timer will be reset.
        ///     This flag is ignored if the mouse pointer is not over the specified window or area.
        /// </summary>
        public const uint HOVER = 0x00000001;

        /// <summary>
        ///     The caller wants leave notification. Notification is delivered as a WM_MOUSELEAVE message.
        ///     If the mouse is not over the specified window or area,
        ///     a leave notification is generated immediately and no further tracking is performed.
        /// </summary>
        public const uint LEAVE = 0x00000002;

        /// <summary>
        ///     The caller wants hover and leave notification for the nonclient areas.
        ///     Notification is delivered as WM_NCMOUSEHOVER and WM_NCMOUSELEAVE messages.
        /// </summary>
        public const uint NONCLIENT = 0x00000010;

        /// <summary>
        ///     The function fills in the structure instead of treating it as a tracking request.
        ///     The structure is filled such that had that structure been passed to TrackMouseEvent,
        ///     it would generate the current tracking.
        ///     The only anomaly is that the hover time-out returned is always the actual time-out and not HOVER_DEFAULT,
        ///     if HOVER_DEFAULT was specified during the original TrackMouseEvent request.
        /// </summary>
        public const uint QUERY = 0x40000000;
    }
}