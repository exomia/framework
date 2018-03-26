#pragma warning disable 1591

using System;
using Exomia.Framework.Game;

namespace Exomia.Framework
{
    /// <summary>
    ///     A render class
    /// </summary>
    public abstract class ARenderer : IComponent, IDrawable, IDisposable
    {
        #region Constructors

        #region Statics

        #endregion

        /// <summary>
        ///     Initializes a new instance of the <see cref="ARenderer" /> class.
        /// </summary>
        /// <param name="name">name</param>
        public ARenderer(string name)
        {
            Name = name;
        }

        #endregion

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
        ///     Gets or sets the name
        /// </summary>
        public string Name { get; } = string.Empty;

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

        #region Methods

        #region Statics

        #endregion

        /// <summary>
        ///     see <see cref="IComponent"></see>
        /// </summary>
        public virtual void Initialize(IServiceRegistry registry) { }

        /// <summary>
        ///     Dispose the renderer
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        ///     see <see cref="IDrawable"></see>
        /// </summary>
        public virtual bool BeginDraw()
        {
            return _visible;
        }

        /// <summary>
        ///     see <see cref="IDrawable"></see>
        /// </summary>
        public virtual void Draw(GameTime gameTime) { }

        /// <summary>
        ///     see <see cref="IDrawable"></see>
        /// </summary>
        public virtual void EndDraw() { }

        #endregion
    }
}