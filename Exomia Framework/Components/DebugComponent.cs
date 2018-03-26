#pragma warning disable 1591

using System;
using System.Diagnostics;
using System.IO;
using Exomia.Framework.ContentSerialization;
using Exomia.Framework.Game;
using Exomia.Framework.Graphics;
using Exomia.Framework.Properties;
using Exomia.Framework.Security;
using SharpDX;

namespace Exomia.Framework.Components
{
    public class DebugComponent : ADrawableComponent
    {
        #region Constructors

        #region Statics

        #endregion

        public DebugComponent(string name = "DebugGameSystem")
            : base(name)
        {
            _totalMemoryBytes = 0;
            _processorLoadT1 = _processorLoadT2 = 0.0f;
            _total_frames = 0;
            _elapsed_time = 0.0f;
            _fpsCurrent = 0.0f;
            _fpsAverage = -1;
        }

        #endregion

        #region Constants

        private const float SAMPLE_TIME_RATE = 2.0f;
        private const int MAXIMUM_SAMPLES = (int)(9 / SAMPLE_TIME_RATE) + 1;
        private const double FRAME_DANGER_THRESHOLD = 1000.0f / 60.0f;

        #endregion

        #region Variables

        #region Statics

        #endregion

        private SpriteFont _arial12px;

        private bool _firstCalc;

        private float _totalMemoryBytes;
        private float _elapsed_time;
        private int _total_frames;
        private float _fpsCurrent;
        private float _fpsAverage;

        private float _processorLoadT1;
        private float _processorLoadT2;

        private string _title = string.Empty;

        private string _cpuInfo = string.Empty;
        private string _ramInfo = string.Empty;
        private string _fpsInfo = string.Empty;
        private string _cpuName = string.Empty;
        private string _gpuName = string.Empty;

        private float _sampleBuffer;
        private int _sampleCount;

        private float _maxFrameTime;

        private IGameWindow _gameWindow;

        private SpriteBatch _spriteBatch;

        private Vector2 _position1;
        private Vector2 _position2;

        private PerformanceCounter _cpuPerformanceCounter1;
        private PerformanceCounter _cpuPerformanceCounter2;
        private PerformanceCounter _ramPerformanceCounter1;

        #endregion

        #region Properties

        #region Statics

        #endregion

        public bool ShowFullInformation { get; set; } = false;

        public bool EnableTitleInformation { get; set; } = false;

        #endregion

        #region Methods

        #region Statics

        #endregion

        protected override void OnInitialize(IServiceRegistry registry)
        {
            IGameWindow gameWindow = registry.GetService<IGameWindow>();

            _title = gameWindow.Title;

            if (gameWindow != null)
            {
                _gameWindow = gameWindow;
            }

            string pName = Process.GetCurrentProcess().ProcessName;
            _cpuPerformanceCounter1 = new PerformanceCounter(nameof(Process), "% Processor Time", pName, true);
            _processorLoadT1 = _cpuPerformanceCounter1.NextValue() / Environment.ProcessorCount;

            _cpuPerformanceCounter2 = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
            _processorLoadT2 = _cpuPerformanceCounter2.NextValue();

            _ramPerformanceCounter1 = new PerformanceCounter(nameof(Process), "Working Set", pName, true);
            _totalMemoryBytes = (long)_ramPerformanceCounter1.NextValue();

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _position1 = new Vector2(10, 20);
            _position2 = new Vector2(10, 80);

            Diagnostic.GetCpuProperty(nameof(Name), out _cpuName);
            Diagnostic.GetGpuProperty(nameof(Name), out _gpuName);
        }

        protected override void OnLoadContent()
        {
            using (MemoryStream ms = new MemoryStream(Resources.arial_ansi_12px))
            {
                if (!ExomiaCryptography.Decrypt(ms, out Stream stream))
                {
                    throw new IOException("resource 'arial_ansi_12px' failed to decrypt");
                }

                _arial12px = ToDispose(ContentSerializer.Read<SpriteFont>(stream)) ??
                             throw new NullReferenceException(nameof(_arial12px));
                if (_arial12px.ImageData == null)
                {
                    throw new NullReferenceException("_arial12px.ImageData");
                }

                using (MemoryStream ms2 = new MemoryStream(_arial12px.ImageData))
                {
                    ms2.Position = 0;
                    _arial12px.Texture = Texture.Load(GraphicsDevice.Device, ms2);
                }
            }
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
                _total_frames = 0;

                _fpsInfo =
                    $"FPS: {_fpsCurrent:0} / {(_fpsAverage == -1 ? "NA" : _fpsAverage.ToString("0"))} ({gameTime.AbsoluteDeltaTimeMS:0.00}ms) [max: {_maxFrameTime:0.00}ms]";
                _fpsInfo = $"{_gpuName}\n{_fpsInfo}";
                _maxFrameTime = 0;
                _firstCalc = true;
            }

            if (_sampleCount >= MAXIMUM_SAMPLES)
            {
                _fpsAverage = _sampleBuffer / _sampleCount;
                _sampleBuffer = 0;
                _sampleCount = 0;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (!_firstCalc) { return; }
            if (EnableTitleInformation)
            {
                _gameWindow.Title = _title + " " + _fpsInfo;
            }

            _spriteBatch.Begin(SpriteSortMode.Deferred);

            if (ShowFullInformation)
            {
                _spriteBatch.DrawText(_arial12px, $"{_cpuInfo}\n\n{_ramInfo}", _position1, Color.White, 0.0f);
            }

            if (_fpsCurrent <= FRAME_DANGER_THRESHOLD)
            {
                _spriteBatch.DrawText(_arial12px, _fpsInfo, _position2, Color.Red, 0.0f);
            }
            else
            {
                _spriteBatch.DrawText(_arial12px, _fpsInfo, _position2, Color.White, 0.0f);
            }

            _spriteBatch.End();
        }

        public override void EndDraw()
        {
            _total_frames++;
            base.EndDraw();
        }

        #endregion
    }
}