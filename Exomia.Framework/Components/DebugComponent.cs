#region MIT License

// Copyright (c) 2019 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

#pragma warning disable 1591

using System;
using System.Diagnostics;
using Exomia.Framework.Content;
using Exomia.Framework.Game;
using Exomia.Framework.Graphics;
using SharpDX;

namespace Exomia.Framework.Components
{
    public class DebugComponent : ADrawableComponent
    {
        private const float SAMPLE_TIME_RATE = 2.0f;
        private const int MAXIMUM_SAMPLES = (int)(9 / SAMPLE_TIME_RATE) + 1;
        private const double FRAME_DANGER_THRESHOLD = 1000.0f / 60.0f;

        private SpriteFont _arial12Px;

        private string _cpuInfo = string.Empty;
        private string _cpuName = string.Empty;

        private PerformanceCounter _cpuPerformanceCounter1;
        private PerformanceCounter _cpuPerformanceCounter2;
        private float _elapsed_time;

        private bool _firstCalc;
        private float _fpsAverage;
        private float _fpsCurrent;
        private string _fpsInfo = string.Empty;

        private IGameWindow _gameWindow;
        private string _gpuName = string.Empty;

        private float _maxFrameTime;

        private Vector2 _position1;
        private Vector2 _position2;

        private float _processorLoadT1;
        private float _processorLoadT2;
        private string _ramInfo = string.Empty;
        private PerformanceCounter _ramPerformanceCounter1;

        private float _sampleBuffer;
        private int _sampleCount;

        private SpriteBatch _spriteBatch;

        private string _title = string.Empty;
        private int _total_frames;

        private float _totalMemoryBytes;

        public bool EnableTitleInformation { get; set; } = false;

        public bool ShowFullInformation { get; set; } = false;

        public DebugComponent(string name = "DebugGameSystem")
            : base(name)
        {
            _totalMemoryBytes = 0;
            _processorLoadT1  = _processorLoadT2 = 0.0f;
            _total_frames     = 0;
            _elapsed_time     = 0.0f;
            _fpsCurrent       = 0.0f;
            _fpsAverage       = -1;
        }

        public override void Draw(GameTime gameTime)
        {
            if (!_firstCalc) { return; }
            if (EnableTitleInformation)
            {
                _gameWindow.Title = _title + " " + _fpsInfo;
            }

            _spriteBatch.Begin();

            if (ShowFullInformation)
            {
                _spriteBatch.DrawText(_arial12Px, $"{_cpuInfo}\n\n{_ramInfo}", _position1, Color.White, 0.0f);
            }

            _spriteBatch.DrawText(
                _arial12Px, _fpsInfo, _position2, _fpsCurrent <= FRAME_DANGER_THRESHOLD ? Color.Red : Color.White,
                0.0f);

            _spriteBatch.End();
        }

        public override void EndDraw()
        {
            _total_frames++;
            base.EndDraw();
        }

        public override void Update(GameTime gameTime)
        {
            _elapsed_time += gameTime.AbsoluteDeltaTimeS;

            if (_maxFrameTime < gameTime.AbsoluteDeltaTimeMS)
            {
                _maxFrameTime = gameTime.AbsoluteDeltaTimeMS;
            }

            if (_elapsed_time >= SAMPLE_TIME_RATE)
            {
                if (ShowFullInformation)
                {
                    _totalMemoryBytes = _ramPerformanceCounter1.NextValue();

                    _processorLoadT1 = _cpuPerformanceCounter1.NextValue() / Environment.ProcessorCount;
                    _processorLoadT2 = _cpuPerformanceCounter2.NextValue();

                    _cpuInfo = $"{_cpuName}\nCPU-Load: {_processorLoadT1:0.0}% Total: {_processorLoadT2:0.0}%";
                    _ramInfo =
                        $"Total Memory Usage: {_totalMemoryBytes / 1024.0:0}KB ({_totalMemoryBytes / 1024.0 / 1024.0:0.00}MB)";
                }

                _fpsCurrent = _total_frames / _elapsed_time;

                _sampleBuffer += _fpsCurrent;
                _sampleCount++;

                _elapsed_time -= SAMPLE_TIME_RATE;
                _total_frames =  0;

                _fpsInfo =

                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    $"FPS: {_fpsCurrent:0} / {(_fpsAverage == -1 ? "NA" : _fpsAverage.ToString("0"))} ({gameTime.AbsoluteDeltaTimeMS:0.00}ms) [max: {_maxFrameTime:0.00}ms]";
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

        protected override void OnLoadContent(IServiceRegistry registry)
        {
            _arial12Px = (registry.GetService<IContentManager>() ??
                          throw new NullReferenceException(nameof(IContentManager)))
                .Load<SpriteFont>("Resources.fonts.arial.arial_12px.e1", true);
        }
    }
}