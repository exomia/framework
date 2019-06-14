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

using System;
using System.IO;
using System.Threading;
using Exomia.Framework.Game;
using Exomia.Framework.Graphics;
using Exomia.Framework.Native;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.MediaFoundation;

namespace Exomia.Framework.Video
{
    /// <summary>
    ///     A video player. This class cannot be inherited.
    /// </summary>
    public sealed class VideoPlayer : ADrawableComponent
    {
        /// <summary>
        ///     The event ready to play.
        /// </summary>
        private readonly ManualResetEvent _eventReadyToPlay = new ManualResetEvent(false);

        /// <summary>
        ///     The output texture.
        /// </summary>
        private readonly Texture2D _outputTexture;

        /// <summary>
        ///     Name of the asset.
        /// </summary>
        private string _assetName;

        /// <summary>
        ///     The background color.
        /// </summary>
        private Color _backgroundColor;

        /// <summary>
        ///     The byte stream.
        /// </summary>
        private ByteStream _byteStream;

        /// <summary>
        ///     Manager for dxgi device.
        /// </summary>
        private DXGIDeviceManager _dxgiDeviceManager;

        /// <summary>
        ///     True if this object is end of stream.
        /// </summary>
        private bool _isEndOfStream;

        /// <summary>
        ///     True if this object is playing.
        /// </summary>
        private bool _isPlaying;

        /// <summary>
        ///     True if this object is video stopped.
        /// </summary>
        private bool _isVideoStopped = true;

        /// <summary>
        ///     The media engine.
        /// </summary>
        private MediaEngine _mediaEngine;

        /// <summary>
        ///     The media engine ex.
        /// </summary>
        private MediaEngineEx _mediaEngineEx;

        /// <summary>
        ///     The sprite batch.
        /// </summary>
        private SpriteBatch _spriteBatch;

        /// <summary>
        ///     The texture.
        /// </summary>
        private Texture _texture;

        /// <summary>
        ///     The graphics device.
        /// </summary>
        private IGraphicsDevice _graphicsDevice;

        /// <summary>
        ///     Gets or sets the name of the asset.
        /// </summary>
        /// <value>
        ///     The name of the asset.
        /// </value>
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

        /// <summary>
        ///     Gets or sets the color of the background.
        /// </summary>
        /// <value>
        ///     The color of the background.
        /// </value>
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

        /// <summary>
        ///     Gets the duration.
        /// </summary>
        /// <value>
        ///     The duration.
        /// </value>
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

        /// <summary>
        ///     Gets or sets a value indicating whether this object is playing.
        /// </summary>
        /// <value>
        ///     True if this object is playing, false if not.
        /// </value>
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

        /// <summary>
        ///     Gets or sets a value indicating whether the mute.
        /// </summary>
        /// <value>
        ///     True if mute, false if not.
        /// </value>
        public bool Mute
        {
            get { return _mediaEngineEx?.Muted ?? false; }
            set { _mediaEngineEx.Muted = value; }
        }

        /// <summary>
        ///     Gets or sets the playback position.
        /// </summary>
        /// <value>
        ///     The playback position.
        /// </value>
        public double PlaybackPosition
        {
            get { return _mediaEngineEx?.CurrentTime ?? 0.0; }
            set { _mediaEngineEx.CurrentTime = value; }
        }

        /// <summary>
        ///     Gets or sets the volume.
        /// </summary>
        /// <value>
        ///     The volume.
        /// </value>
        public double Volume
        {
            get { return _mediaEngineEx?.Volume ?? 0.0; }
            set { _mediaEngineEx.Volume = value; }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="VideoPlayer" /> class.
        /// </summary>
        /// <param name="device"> The device. </param>
        /// <param name="width">  The width. </param>
        /// <param name="height"> The height. </param>
        public VideoPlayer(Device5 device, int width, int height)
            : base(nameof(VideoPlayer))
        {
            _outputTexture   = TextureHelper.CreateTexture(device, width, height);
            _backgroundColor = Color.Transparent;
        }

        /// <inheritdoc />
        public override bool BeginDraw()
        {
            return base.BeginDraw() && !_isVideoStopped && _isInitialized;
        }

        /// <inheritdoc />
        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_texture, Vector2.Zero, Color.White);
            _spriteBatch.End();
        }

        /// <summary>
        ///     Pauses this object.
        /// </summary>
        public void Pause()
        {
            _mediaEngineEx?.Pause();
        }

        /// <summary>
        ///     Plays this object.
        /// </summary>
        public void Play()
        {
            if (_mediaEngineEx != null)
            {
                if (_mediaEngineEx.HasVideo() && _isVideoStopped) { _isVideoStopped = false; }

                if (_isEndOfStream)
                {
                    PlaybackPosition = 0;
                    _isPlaying       = true;
                }
                else
                {
                    _mediaEngineEx.Play();
                }

                _isEndOfStream = false;
            }
        }

        /// <summary>
        ///     Sets byte stream.
        /// </summary>
        /// <param name="stream"> The stream. </param>
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
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

        /// <summary>
        ///     Shuts down this object and frees any resources it is using.
        /// </summary>
        public void Shutdown()
        {
            if (_isInitialized)
            {
                Stop();

                _mediaEngineEx?.Shutdown();
            }
        }

        /// <inheritdoc />
        public override void Update(GameTime gameTime)
        {
            if (_isVideoStopped || !_isInitialized) { return; }

            if (_mediaEngineEx != null)
            {
                if (_mediaEngineEx.OnVideoStreamTick(out _))
                {
                    _mediaEngineEx.TransferVideoFrame(
                        _outputTexture,
                        null,
                        new Rectangle(0, 0, _outputTexture.Description.Width, _outputTexture.Description.Height),
                        (ColorBGRA)BackgroundColor);

                    //TODO: draw the texture directly to the _texture's TexturePointer instead of creating every time new one
                    _texture = new Texture(
                        new ShaderResourceView1(_graphicsDevice.Device, _outputTexture),
                        _outputTexture.Description.Width, _outputTexture.Description.Height);
                }
            }
        }

        /// <inheritdoc />
        protected override void OnInitialize(IServiceRegistry registry)
        {
            _graphicsDevice = registry.GetService<IGraphicsDevice>() ??
                              throw new NullReferenceException(nameof(IGraphicsDevice));

            _spriteBatch = ToDispose(new SpriteBatch(_graphicsDevice));
            MediaManager.Startup();

            DeviceMultithread multithread = _graphicsDevice.Device.QueryInterface<DeviceMultithread>();
            multithread.SetMultithreadProtected(true);

            _dxgiDeviceManager = ToDispose(new DXGIDeviceManager());
            _dxgiDeviceManager.ResetDevice(_graphicsDevice.Device);

            MediaEngineAttributes attributes = new MediaEngineAttributes
            {
                DxgiManager = _dxgiDeviceManager, VideoOutputFormat = (int)Format.B8G8R8A8_UNorm
            };

            using (MediaEngineClassFactory factory = new MediaEngineClassFactory())
            {
                _mediaEngine = ToDispose(
                    new MediaEngine(
                        factory, attributes, MediaEngineCreateFlags.WaitForStableState, OnMediaEngineEvent));
            }
            _mediaEngineEx = ToDispose(_mediaEngine.QueryInterface<MediaEngineEx>());
        }

        /// <summary>
        ///     Executes the media engine event action.
        /// </summary>
        /// <param name="mediaEvent"> The media event. </param>
        /// <param name="param1">     The first parameter. </param>
        /// <param name="param2">     The second parameter. </param>
        private void OnMediaEngineEvent(MediaEngineEvent mediaEvent, long param1, int param2)
        {
            switch (mediaEvent)
            {
                case MediaEngineEvent.NotifyStableState:
                    Kernel32.SetEvent(new IntPtr(param1));
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

        /// <summary>
        ///     Stops this object.
        /// </summary>
        private void Stop()
        {
            _isVideoStopped = true;
            _isPlaying      = false;
        }
    }
}