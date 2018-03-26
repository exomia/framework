using System;
using Exomia.Framework.Game;

namespace Exomia.Framework
{
    /// <summary>
    ///     An interface to update a game component
    /// </summary>
    public interface IUpdateable
    {
        /// <summary>
        ///     Gets a value indicating whether the game component's Update method should be called.
        /// </summary>
        /// <value><c>true</c> if update is enabled; otherwise, <c>false</c>.</value>
        bool Enabled { get; set; }

        /// <summary>
        ///     Gets or sets the update order relative to other game components. Lower values are updated first.
        /// </summary>
        /// <value>The update order.</value>
        int UpdateOrder { get; set; }

        /// <summary>
        ///     Occurs when the <see cref="UpdateOrder" /> property changes.
        /// </summary>
        event EventHandler<EventArgs> UpdateOrderChanged;

        /// <summary>
        ///     Occurs when the <see cref="Enabled" /> property changes.
        /// </summary>
        event EventHandler<EventArgs> EnabledChanged;

        /// <summary>
        ///     This method is called when this game component is updated.
        /// </summary>
        /// <param name="gameTime">The current timing.</param>
        void Update(GameTime gameTime);
    }
}