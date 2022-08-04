#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.ContentManager.Extensions;

/// <summary>
///     A control extensions.
/// </summary>
static class ControlExtensions
{
    /// <summary>
    ///     A Control extension method that executes if required on a different thread, and waits for the result.
    /// </summary>
    /// <typeparam name="TControl"> Type of the control. </typeparam>
    /// <param name="control"> The control to act on. </param>
    /// <param name="action"> The action. </param>
    public static void InvokeIfRequired<TControl>(this TControl control, Action<TControl> action)
        where TControl : Control
    {
        if (control.InvokeRequired)
        {
            control.Invoke(action, control);
        }
        else
        {
            action(control);
        }
    }

    /// <summary>
    ///     A Control extension method that executes if required on a different thread, and waits for the result.
    /// </summary>
    /// <typeparam name="TControl"> Generic type parameter. </typeparam>
    /// <typeparam name="TResult"> Type of the result. </typeparam>
    /// <param name="control"> The control to act on. </param>
    /// <param name="func"> The function. </param>
    /// <returns>
    ///     A T.
    /// </returns>
    public static TResult InvokeIfRequired<TControl, TResult>(this TControl control, Func<TControl, TResult> func)
        where TControl : Control
    {
        if (control.InvokeRequired)
        {
            return (TResult)control.Invoke(func, control);
        }
        return func(control);
    }

    /// <summary>
    ///     A Control extension method that executes if required on a different thread, and waits for the result.
    /// </summary>
    /// <typeparam name="TControl"> Type of the control. </typeparam>
    /// <param name="action"> The action. </param>
    /// <param name="controls"> A variable-length parameters list containing controls. </param>
    public static void InvokeIfRequiredOn<TControl>(Action<TControl> action, params TControl[] controls)
        where TControl : Control
    {
        foreach (TControl control in controls)
        {
            control.InvokeIfRequired(action);
        }
    }
}