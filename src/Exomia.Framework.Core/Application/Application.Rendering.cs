#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Application;

public abstract partial class Application
{
    /// <summary>
    ///     Starts the rendering of a frame.
    ///     This method is followed by calls to <see cref="Render" /> and <see cref="EndFrame" />.
    /// </summary>
    /// <returns> <c> true </c> if Draw should occur, <c> false </c> otherwise. </returns>
    protected abstract bool BeginFrame();

    /// <summary> Perform rendering operations. </summary>
    /// <param name="time"> The current timing. </param>
    protected virtual void Render(Time time)
    {
        lock (_renderableComponents)
        {
            _currentlyRenderableComponents.AddRange(_renderableComponents);
        }

        for (int i = 0; i < _currentlyRenderableComponents.Count; i++)
        {
            IRenderable renderable = _currentlyRenderableComponents[i];
            if (renderable.BeginFrame())
            {
                renderable.Render(time);
                renderable.EndFrame();
            }
        }

        _currentlyRenderableComponents.Clear();
    }

    /// <summary>
    ///     Ends the rendering of a frame.
    ///     This method is preceded by calls to <see cref="Render" /> and <see cref="BeginFrame" />.
    /// </summary>
    protected abstract void EndFrame();
}