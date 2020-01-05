#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Diagnostics;
using Exomia.Framework.Content;
using Exomia.Framework.Game;
using Exomia.Framework.Graphics;
using SharpDX;

namespace Exomia.Framework.Components
{
    /// <summary>
    ///     A debug component.
    /// </summary>
    public class DebugComponent : DrawableComponent
    {
        /// <summary>
        ///     The sample time rate.
        /// </summary>
        private const float SAMPLE_TIME_RATE = 2.0f;

        /// <summary>
        ///     The maximum samples.
        /// </summary>
        private const int MAXIMUM_SAMPLES = (int)(9 / SAMPLE_TIME_RATE) + 1;

        /// <summary>
        ///     The frame danger threshold.
        /// </summary>
        private const double FRAME_DANGER_THRESHOLD = 1000.0f / 60.0f;

        /// <summary>
        ///     The arial 12 px.
        /// </summary>
        private SpriteFont? _arial12Px;

        /// <summary>
        ///     Information describing the CPU.
        /// </summary>
        private string _cpuInfo = string.Empty;

        /// <summary>
        ///     Name of the CPU.
        /// </summary>
        private string _cpuName = string.Empty;

        /// <summary>
        ///     The first CPU performance counter.
        /// </summary>
        private PerformanceCounter? _cpuPerformanceCounter1;

        /// <summary>
        ///     The second CPU performance counter.
        /// </summary>
        private PerformanceCounter? _cpuPerformanceCounter2;

        /// <summary>
        ///     The elapsed time.
        /// </summary>
        private float _elapsedTime;

        /// <summary>
        ///     True to first calculate.
        /// </summary>
        private bool _firstCalc;

        /// <summary>
        ///     The FPS average.
        /// </summary>
        private float _fpsAverage;

        /// <summary>
        ///     The FPS current.
        /// </summary>
        private float _fpsCurrent;

        /// <summary>
        ///     Information describing the FPS.
        /// </summary>
        private string _fpsInfo = string.Empty;

        /// <summary>
        ///     The game window.
        /// </summary>
        private IGameWindow? _gameWindow;

        /// <summary>
        ///     Name of the GPU.
        /// </summary>
        private string _gpuName = string.Empty;

        /// <summary>
        ///     The maximum frame time.
        /// </summary>
        private float _maxFrameTime;

        /// <summary>
        ///     The first position.
        /// </summary>
        private Vector2 _position1;

        /// <summary>
        ///     The second position.
        /// </summary>
        private Vector2 _position2;

        /// <summary>
        ///     The first processor load t.
        /// </summary>
        private float _processorLoadT1;

        /// <summary>
        ///     The second processor load t.
        /// </summary>
        private float _processorLoadT2;

        /// <summary>
        ///     Information describing the ram.
        /// </summary>
        private string _ramInfo = string.Empty;

        /// <summary>
        ///     The first ram performance counter.
        /// </summary>
        private PerformanceCounter? _ramPerformanceCounter1;

        /// <summary>
        ///     Buffer for sample data.
        /// </summary>
        private float _sampleBuffer;

        /// <summary>
        ///     Number of samples.
        /// </summary>
        private int _sampleCount;

        /// <summary>
        ///     The sprite batch.
        /// </summary>
        private SpriteBatch? _spriteBatch;

        /// <summary>
        ///     The title.
        /// </summary>
        private string _title = string.Empty;

        /// <summary>
        ///     The total frames.
        /// </summary>
        private int _totalFrames;

        /// <summary>
        ///     The total memory in bytes.
        /// </summary>
        private float _totalMemoryBytes;

        /// <summary>
        ///     Gets or sets a value indicating whether the title information is enabled.
        /// </summary>
        /// <value>
        ///     True if enable title information, false if not.
        /// </value>
        public bool EnableTitleInformation { get; set; } = false;

        /// <summary>
        ///     Gets or sets a value indicating whether the full information is shown.
        /// </summary>
        /// <value>
        ///     True if show full information, false if not.
        /// </value>
        public bool ShowFullInformation { get; set; } = false;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DebugComponent" /> class.
        /// </summary>
        /// <param name="name"> (Optional) The name. </param>
        public DebugComponent(string name = "DebugGameSystem")
            : base(name)
        {
            _totalMemoryBytes = 0;
            _processorLoadT1  = _processorLoadT2 = 0.0f;
            _totalFrames      = 0;
            _elapsedTime      = 0.0f;
            _fpsCurrent       = 0.0f;
            _fpsAverage       = -1;
        }

        /// <inheritdoc />
        public override void Draw(GameTime gameTime)
        {
            if (!_firstCalc) { return; }
            if (EnableTitleInformation)
            {
                _gameWindow!.Title = _title + " " + _fpsInfo;
            }

            _spriteBatch!.Begin();

            if (ShowFullInformation)
            {
                _spriteBatch.DrawText(_arial12Px!, $"{_cpuInfo}\n\n{_ramInfo}", _position1, Color.White, 0.0f);
            }

            _spriteBatch.DrawText(
                _arial12Px!, _fpsInfo, _position2, _fpsCurrent <= FRAME_DANGER_THRESHOLD ? Color.Red : Color.White,
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
                if (ShowFullInformation)
                {
                    _totalMemoryBytes = _ramPerformanceCounter1!.NextValue();

                    _processorLoadT1 = _cpuPerformanceCounter1!.NextValue() / Environment.ProcessorCount;
                    _processorLoadT2 = _cpuPerformanceCounter2!.NextValue();

                    _cpuInfo = $"{_cpuName}\nCPU-Load: {_processorLoadT1:0.0}% Total: {_processorLoadT2:0.0}%";
                    _ramInfo =
                        $"Total Memory Usage: {_totalMemoryBytes / 1024.0:0}KB ({_totalMemoryBytes / 1024.0 / 1024.0:0.00}MB)";
                }

                _fpsCurrent = _totalFrames / _elapsedTime;

                _sampleBuffer += _fpsCurrent;
                _sampleCount++;

                _elapsedTime -= SAMPLE_TIME_RATE;
                _totalFrames =  0;

                _fpsInfo =

                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    $"FPS: {_fpsCurrent:0} / {(_fpsAverage == -1 ? "NA" : _fpsAverage.ToString("0"))} ({gameTime.DeltaTimeMS:0.00}ms) [max: {_maxFrameTime:0.00}ms]";
                _fpsInfo      = $"{_gpuName}\n{_fpsInfo}";
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

            string pName = Process.GetCurrentProcess().ProcessName;
            _cpuPerformanceCounter1 = new PerformanceCounter(nameof(Process), "% Processor Time", pName, true);
            _processorLoadT1        = _cpuPerformanceCounter1.NextValue() / Environment.ProcessorCount;

            _cpuPerformanceCounter2 = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
            _processorLoadT2        = _cpuPerformanceCounter2.NextValue();

            _ramPerformanceCounter1 = new PerformanceCounter(nameof(Process), "Working Set", pName, true);
            _totalMemoryBytes       = (long)_ramPerformanceCounter1.NextValue();

            _spriteBatch = new SpriteBatch(
                registry.GetService<IGraphicsDevice>() ?? throw new NullReferenceException(nameof(IGraphicsDevice)));

            _position1 = new Vector2(10, 20);
            _position2 = new Vector2(10, 80);

            Diagnostic.Diagnostic.GetCpuProperty(nameof(Name), out _cpuName);
            Diagnostic.Diagnostic.GetGpuProperty(nameof(Name), out _gpuName);
        }

        /// <inheritdoc />
        protected override void OnLoadContent(IServiceRegistry registry)
        {
            _arial12Px = (registry.GetService<IContentManager>() ??
                          throw new NullReferenceException(nameof(IContentManager)))
                .Load<SpriteFont>("Resources.fonts.arial.arial_12px.e1", true);
        }
    }
}