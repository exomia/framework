#pragma warning disable 1591

using System;
using Exomia.Framework.Game;

namespace Exomia.Framework
{
    /// <summary>
    ///     A drawable game component
    /// </summary>
    public abstract class ADrawableComponent : AComponent, IDrawable
    {
        #region Constants

        #endregion

        #region Variables

        #region Statics

        #endregion

        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;

        private bool _visible;
        private int _drawOrder;

        #endregion

        #region Properties

        #region Statics

        #endregion

        /// <summary>
        ///     Gets or sets the visible state
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    VisibleChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the draw order
        /// </summary>
        public int DrawOrder
        {
            get { return _drawOrder; }
            set
            {
                if (_drawOrder != value)
                {
                    _drawOrder = value;
                    DrawOrderChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region Constructors

        #region Statics

        #endregion

        /// <summary>
        ///     Initializes a new instance of the <see cref="ADrawableComponent" /> class.
        /// </summary>
        /// <param name="name">name</param>
        public ADrawableComponent(string name)
            : base(name) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ADrawableComponent" /> class.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="name">name</param>
        public ADrawableComponent(Game.Game game, string name)
            : base(game, name) { }

        #endregion

        #region Methods

        #region Statics

        #endregion

        /// <summary>
        ///     see <see cref="IDrawable.BeginDraw()"></see>
        /// </summary>
        public virtual bool BeginDraw()
        {
            return _visible;
        }

        /// <summary>
        ///     see <see cref="IDrawable.Draw(GameTime)"></see>
        /// </summary>
        public abstract void Draw(GameTime gameTime);

        /// <summary>
        ///     see <see cref="IDrawable.EndDraw()"></see>
        /// </summary>
        public virtual void EndDraw() { }

        #endregion
    }
}