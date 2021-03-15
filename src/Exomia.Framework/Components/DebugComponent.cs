#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;
using Exomia.Framework.Content;
using Exomia.Framework.Game;
using Exomia.Framework.Graphics;
using Exomia.Framework.Resources;
using Exomia.Vulkan.Api.Core;

namespace Exomia.Framework.Components
{
    /// <summary>
    ///     A debug component.
    /// </summary>
    public class DebugComponent : DrawableComponent
    {
        private const float       SAMPLE_TIME_RATE       = 2.0f;
        private const int         MAXIMUM_SAMPLES        = (int)(9 / SAMPLE_TIME_RATE) + 1;
        private const double      FRAME_DANGER_THRESHOLD = 1000.0f / 60.0f;
        private       SpriteFont? _arial12Px;

        private string _fpsInfo = string.Empty,
                       _gpuName = string.Empty;

        private bool _firstCalc;

        private float _fpsAverage,
                      _fpsCurrent,
                      _elapsedTime,
                      _maxFrameTime,
                      _sampleBuffer;

        private IGameWindow? _gameWindow;
        private int          _sampleCount, _totalFrames;
        private SpriteBatch? _spriteBatch;
        private string       _title = string.Empty;

        /// <summary>
        ///     Gets or sets a value indicating whether the title information is enabled.
        /// </summary>
        /// <value>
        ///     True if enable title information, false if not.
        /// </value>
        public bool EnableTitleInformation { get; set; } = false;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DebugComponent" /> class.
        /// </summary>
        /// <param name="name"> (Optional) The name. </param>
        public DebugComponent(string name = "DebugGameSystem")
            : base(name)
        {
            _totalFrames = 0;
            _elapsedTime = 0.0f;
            _fpsCurrent  = 0.0f;
            _fpsAverage  = -1;
        }

        /// <inheritdoc />
        public override void Draw(GameTime gameTime)
        {
            if (!_firstCalc) { return; }

            if (EnableTitleInformation)
            {
                _gameWindow!.Title = $"{_title} {_fpsInfo}";
            }

            _spriteBatch!.Begin();

            _spriteBatch.DrawText(
                _arial12Px!,
                _fpsInfo,
                Vector2.Zero,
                _fpsCurrent <= FRAME_DANGER_THRESHOLD
                    ? VkColor.Red
                    : VkColor.White,
                0.0f);

            _spriteBatch.End();
        }

        /// <inheritdoc />
        public override void EndDraw()
        {
            _totalFrames++;
            base.EndDraw();
        }

        /// <inheritdoc />
        public override void Update(GameTime gameTime)
        {
            _elapsedTime += gameTime.DeltaTimeS;

            if (_maxFrameTime < gameTime.DeltaTimeMS)
            {
                _maxFrameTime = gameTime.DeltaTimeMS;
            }

            if (_elapsedTime >= SAMPLE_TIME_RATE)
            {
                _fpsCurrent = _totalFrames / _elapsedTime;

                _sampleBuffer += _fpsCurrent;
                _sampleCount++;

                _elapsedTime -= SAMPLE_TIME_RATE;
                _totalFrames =  0;

                _fpsInfo =

                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    $"{_gpuName}\nFPS: {_fpsCurrent:0} / {(_fpsAverage == -1 ? "NA" : _fpsAverage.ToString("0"))} ({gameTime.DeltaTimeMS:0.00}ms) [max: {_maxFrameTime:0.00}ms]";
                _maxFrameTime = 0;
                _firstCalc    = true;
            }

            if (_sampleCount >= MAXIMUM_SAMPLES)
            {
                _fpsAverage   = _sampleBuffer / _sampleCount;
                _sampleBuffer = 0;
                _sampleCount  = 0;
            }
        }

        /// <inheritdoc />
        protected override void OnInitialize(IServiceRegistry registry)
        {
            IGameWindow gameWindow = registry.GetService<IGameWindow>();
            _title      = gameWindow.Title;
            _gameWindow = gameWindow;

            IGraphicsDevice graphicsDevice = registry.GetService<IGraphicsDevice>();
            _spriteBatch = new SpriteBatch(graphicsDevice);

            _gpuName = graphicsDevice.Adapter?.Desc3.Description ?? "<unknown>";
        }

        /// <inheritdoc />
        protected override void OnLoadContent(IServiceRegistry registry)
        {
            _arial12Px = registry.GetService<IContentManager>()
                                 .Load<SpriteFont>(Fonts.ARIAL_12_PX, true);
        }
    }
}