#region MIT License

// Copyright (c) 2018 exomia - Daniel Bätz
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
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Exomia.Framework.Game;
using Exomia.Framework.Graphics;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.MediaFoundation;

namespace Exomia.Framework.Video
{
    public sealed class VideoPlayer : ADrawableComponent
    {
        private readonly ManualResetEvent _eventReadyToPlay = new ManualResetEvent(false);
        private readonly Texture2D _outputTexture;
        private string _assetName;
        private Color _backgroundColor;
        private ByteStream _byteStream;
        private DXGIDeviceManager _dxgiDeviceManager;
        private bool _isEndOfStream;
        private bool _isPlaying;
        private bool _isVideoStopped = true;
        private MediaEngine _mediaEngine;
        private MediaEngineEx _mediaEngineEx;
        private SpriteBatch _spriteBatch;
        private Texture _texture;

        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                if (_backgroundColor != value)
                {
                    _backgroundColor = value;
                }
            }
        }

        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;
                }
            }
        }

        public string AssetName
        {
            get { return _assetName; }
            set
            {
                if (_assetName != value)
                {
                    _assetName = value;
                }
            }
        }

        public double PlaybackPosition
        {
            get { return _mediaEngineEx?.CurrentTime ?? 0.0; }
            set
            {
                if (_mediaEngineEx != null) { _mediaEngineEx.CurrentTime = value; }
            }
        }

        public double Duration
        {
            get
            {
                double duration = 0.0;
                if (_mediaEngineEx != null)
                {
                    duration = _mediaEngineEx.Duration;
                    if (double.IsNaN(duration)) { duration = 0.0; }
                }
                return duration;
            }
        }

        public bool Mute
        {
            get
            {
                if (_mediaEngineEx != null) { return _mediaEngineEx.Muted; }
                return false;
            }
            set
            {
                if (_mediaEngineEx != null) { _mediaEngineEx.Muted = value; }
            }
        }

        public double Volume
        {
            get { return _mediaEngineEx?.Volume ?? 0.0; }
            set
            {
                if (_mediaEngineEx != null) { _mediaEngineEx.Volume = value; }
            }
        }

        public VideoPlayer(Device5 device, int width, int height)
            : base(nameof(VideoPlayer))
        {
            _outputTexture = TextureHelper.CreateTexture(device, width, height);
            _backgroundColor = Color.Transparent;
        }

        [DllImport("kernel32.dll", EntryPoint = "SetEvent")]
        private static extern bool SetEvent(IntPtr hEvent);

        public override void Update(GameTime gameTime)
        {
            if (_isVideoStopped || !_isInitialized) { return; }

            if (_mediaEngineEx != null)
            {
                if (_mediaEngineEx.OnVideoStreamTick(out long pts))
                {
                    _mediaEngineEx.TransferVideoFrame(
                        _outputTexture,
                        null,
                        new Rectangle(0, 0, _outputTexture.Description.Width, _outputTexture.Description.Height),
                        (ColorBGRA)BackgroundColor);

                    _texture = new Texture(
                        new ShaderResourceView1(GraphicsDevice.Device, _outputTexture),
                        _outputTexture.Description.Width, _outputTexture.Description.Height);
                }
            }
        }

        public override bool BeginDraw()
        {
            return base.BeginDraw() && !_isVideoStopped && _isInitialized;
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_texture, Vector2.Zero, Color.White);
            _spriteBatch.End();
        }

        public void SetByteStream(Stream stream)
        {
            _byteStream = new ByteStream(stream);
            if (_mediaEngineEx != null)
            {
                Uri url = new Uri(_assetName, UriKind.RelativeOrAbsolute);
                _mediaEngineEx.SetSourceFromByteStream(_byteStream, url.AbsoluteUri);
                if (!_eventReadyToPlay.WaitOne(5000))
                {
                    throw new Exception("Unexpected error: Unable to play this file");
                }
            }
        }

        public void Play()
        {
            if (_mediaEngineEx != null)
            {
                if (_mediaEngineEx.HasVideo() && _isVideoStopped) { _isVideoStopped = false; }

                if (_isEndOfStream)
                {
                    PlaybackPosition = 0;
                    _isPlaying = true;
                }
                else
                {
                    _mediaEngineEx.Play();
                }

                _isEndOfStream = false;
            }
        }

        public void Pause()
        {
            _mediaEngineEx?.Pause();
        }

        public void Shutdown()
        {
            if (_isInitialized)
            {
                Stop();

                _mediaEngineEx?.Shutdown();
            }
        }

        protected override void OnInitialize(IServiceRegistry registry)
        {
            _spriteBatch = ToDispose(new SpriteBatch(GraphicsDevice));
            MediaManager.Startup();

            DeviceMultithread multithread = GraphicsDevice.Device.QueryInterface<DeviceMultithread>();
            multithread.SetMultithreadProtected(true);

            _dxgiDeviceManager = ToDispose(new DXGIDeviceManager());
            _dxgiDeviceManager.ResetDevice(GraphicsDevice.Device);

            MediaEngineAttributes attributes = new MediaEngineAttributes
            {
                DxgiManager = _dxgiDeviceManager,
                VideoOutputFormat = (int)Format.B8G8R8A8_UNorm
            };

            using (MediaEngineClassFactory factory = new MediaEngineClassFactory())
            {
                _mediaEngine = ToDispose(
                    new MediaEngine(
                        factory, attributes, MediaEngineCreateFlags.WaitForStableState, OnMediaEngineEvent));
            }
            _mediaEngineEx = ToDispose(_mediaEngine.QueryInterface<MediaEngineEx>());
        }

        private void Stop()
        {
            _isVideoStopped = true;
            _isPlaying = false;
        }

        private void OnMediaEngineEvent(MediaEngineEvent mediaEvent, long param1, int param2)
        {
            switch (mediaEvent)
            {
                case MediaEngineEvent.NotifyStableState:
                    SetEvent(new IntPtr(param1));
                    break;
                case MediaEngineEvent.LoadedMetadata:
                    _isEndOfStream = false;
                    break;
                case MediaEngineEvent.CanPlay:
                    _eventReadyToPlay.Set();
                    break;
                case MediaEngineEvent.Play:
                    _isPlaying = true;
                    break;
                case MediaEngineEvent.Pause:
                    _isPlaying = false;
                    break;
                case MediaEngineEvent.Ended:
                    if (_mediaEngineEx.HasVideo())
                    {
                        Stop();
                    }
                    _isEndOfStream = true;
                    break;
                case MediaEngineEvent.TimeUpdate:
                    break;
                case MediaEngineEvent.Error:
                    break;
            }
        }
    }
}