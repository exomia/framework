using System;
using System.Collections.Generic;
using System.IO;
using SharpDX;
using SharpDX.Multimedia;
using SharpDX.X3DAudio;
using SharpDX.XAudio2;

namespace Exomia.Framework.Audio
{
    /// <summary>
    ///     AudioManager class
    /// </summary>
    public sealed class AudioManager : IDisposable
    {
        #region Constants

        #endregion

        #region Variables

        #region Statics

        #endregion

        private readonly int _inputChannelCount = 2;
        private readonly int _inputSampleRate = 44100;

        private float _masterVolume = 0.5f;
        private float _bgmVolume = 1.0f;
        private float _fxVolume = 1.0f;
        private float _envVolume = 1.0f;

        private XAudio2 _xAudio2;
        private X3DAudio _x3dAudio;
        private MasteringVoice _masteringVoice;

        private VoiceSendDescriptor _fxVoiceSendDescriptor;
        private SubmixVoice _fxSubmixVoice;
        private VoiceSendDescriptor _envVoiceSendDescriptor;
        private SubmixVoice _envSubmixVoice;

        private SourceVoice _currentBGM;

        private struct SoundBuffer
        {
            public AudioBuffer AudioBuffer;
            public WaveFormat Format;
            public uint[] DecodedPacketsInfo;
        }

        private Dictionary<int, SoundBuffer> _soundBuffer;
        private int _soundBufferIndex;

        private LinkedSoundList _fxLinkedSoundList;
        private LinkedSoundList _envLinkedSoundList;

        private readonly Listener _listener;

        #endregion

        #region Properties

        #region Statics

        #endregion

        /// <summary>
        ///     ListenerPosition
        /// </summary>
        public Vector3 ListenerPosition
        {
            get { return _listener.Position; }
            set
            {
                _listener.Position = value;
                RecalculateFXSounds();
                RecalculateEnvSounds();
            }
        }

        /// <summary>
        ///     ListenerVelocity
        /// </summary>
        public Vector3 ListenerVelocity
        {
            get { return _listener.Velocity; }
            set
            {
                _listener.Velocity = value;
                RecalculateFXSounds();
                RecalculateEnvSounds();
            }
        }

        /// <summary>
        ///     MasterVolume
        /// </summary>
        public float MasterVolume
        {
            get { return _masterVolume; }
            set
            {
                _masterVolume = MathUtil.Clamp(value, 0.0f, 1.0f);
                _masteringVoice?.SetVolume(_masterVolume);
            }
        }

        /// <summary>
        ///     BGMVolume
        /// </summary>
        public float BGMVolume
        {
            get { return _bgmVolume; }
            set
            {
                _bgmVolume = MathUtil.Clamp(value, 0.0f, 1.0f);
                if (_currentBGM != null && _currentBGM.State.BuffersQueued > 0)
                {
                    _currentBGM.SetVolume(_bgmVolume);
                }
            }
        }

        /// <summary>
        ///     FXVolume
        /// </summary>
        public float FXVolume
        {
            get { return _fxVolume; }
            set
            {
                _fxVolume = MathUtil.Clamp(value, 0.0f, 1.0f);
                _fxSubmixVoice?.SetVolume(_fxVolume);
            }
        }

        /// <summary>
        ///     EnvironmentVolume
        /// </summary>
        public float EnvironmentVolume
        {
            get { return _envVolume; }
            set
            {
                _envVolume = MathUtil.Clamp(value, 0.0f, 1.0f);
                _envSubmixVoice?.SetVolume(_envVolume);
            }
        }

        #endregion

        #region Constructors

        #region Statics

        #endregion

        /// <summary>
        ///     Initializes a new instance of the <see cref="AudioManager" /> class.
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="fxSoundPoolLimit"></param>
        /// <param name="speakers"></param>
        /// <param name="deviceID"></param>
        public AudioManager(Listener listener, int fxSoundPoolLimit, Speakers speakers, string deviceID = null)
        {
            _listener = listener;
            if (fxSoundPoolLimit <= 0)
            {
                throw new ArgumentException("fxSoundPoolLimit must be bigger than 0");
            }
            _soundBuffer = new Dictionary<int, SoundBuffer>(128);
            _fxLinkedSoundList = new LinkedSoundList(fxSoundPoolLimit);
            _envLinkedSoundList = new LinkedSoundList(int.MaxValue);

#if DEBUG
            _xAudio2 = new XAudio2(XAudio2Flags.DebugEngine, ProcessorSpecifier.AnyProcessor, XAudio2Version.Default);
#else
            _xAudio2 = new XAudio2(XAudio2Flags.None, ProcessorSpecifier.AnyProcessor, XAudio2Version.Default);
#endif
            if (_xAudio2.Version == XAudio2Version.Version27)
            {
                if (int.TryParse(deviceID, out int deviceID27))
                {
                    _masteringVoice = new MasteringVoice(
                        _xAudio2, XAudio2.DefaultChannels, XAudio2.DefaultSampleRate, deviceID27);
                }
            }
            else
            {
                _masteringVoice = new MasteringVoice(
                    _xAudio2, XAudio2.DefaultChannels, XAudio2.DefaultSampleRate, deviceID);
            }
            if (_masteringVoice == null)
            {
                throw new Exception("can't create MasteringVoice");
            }
            _masteringVoice.SetVolume(_masterVolume);
            _x3dAudio = new X3DAudio(speakers, X3DAudio.SpeedOfSound / 1000f, X3DAudioVersion.Default);

            _masteringVoice.GetVoiceDetails(out VoiceDetails details);
            _inputChannelCount = details.InputChannelCount;
            _inputSampleRate = details.InputSampleRate;

            _fxSubmixVoice = new SubmixVoice(_xAudio2, _inputChannelCount, _inputSampleRate);
            _fxSubmixVoice.SetVolume(_fxVolume);

            _envSubmixVoice = new SubmixVoice(_xAudio2, _inputChannelCount, _inputSampleRate);
            _envSubmixVoice.SetVolume(_envVolume);

            _fxVoiceSendDescriptor = new VoiceSendDescriptor(VoiceSendFlags.None, _fxSubmixVoice);
            _envVoiceSendDescriptor = new VoiceSendDescriptor(VoiceSendFlags.None, _envSubmixVoice);
        }

        /// <summary>
        ///     destructor
        /// </summary>
        ~AudioManager()
        {
            Dispose(false);
        }

        #endregion

        #region Methods

        #region Statics

        #endregion

        /// <summary>
        ///     load a sound from a given stream and returns the sound id
        /// </summary>
        /// <param name="stream">soundstream to load the sound from</param>
        /// <returns>sound id</returns>
        public int LoadSound(Stream stream)
        {
            using (SoundStream soundStream = new SoundStream(stream))
            {
                AudioBuffer audioBuffer = new AudioBuffer
                {
                    Stream = soundStream.ToDataStream(),
                    AudioBytes = (int)soundStream.Length,
                    Flags = BufferFlags.EndOfStream
                };
                soundStream.Close();
                _soundBuffer.Add(
                    _soundBufferIndex, new SoundBuffer
                    {
                        AudioBuffer = audioBuffer,
                        Format = soundStream.Format,
                        DecodedPacketsInfo = soundStream.DecodedPacketsInfo
                    });
            }
            return _soundBufferIndex++;
        }

        /// <summary>
        ///     unload a sound and free resources from a given sound id
        /// </summary>
        /// <param name="soundID">the sound id</param>
        /// <returns><c>true</c>if sound was deleted successfully; <c>false</c> otherwise.</returns>
        public bool UnloadSound(int soundID)
        {
            return _soundBuffer.Remove(soundID);
        }

        /// <summary>
        ///     unload all loaded sounds and free resources
        /// </summary>
        public void UnloadAll()
        {
            StopBGM();
            _soundBuffer.Clear();
            _soundBufferIndex = 0;
        }

        /// <summary>
        ///     play a fx sound
        /// </summary>
        /// <param name="soundID">the sound id</param>
        /// <param name="emitterPos">emitter position</param>
        /// <param name="volume">volume</param>
        /// <param name="maxDistance">max distance to hear the sound</param>
        /// <param name="onFXEnd">called than the sound ends</param>
        public void PlayFxSound(int soundID, Vector3 emitterPos, float volume, float maxDistance,
            Action<IntPtr> onFXEnd = null)
        {
            if (_fxLinkedSoundList.Count >= _fxLinkedSoundList.Capacity)
            {
                return;
            }
            PlaySound(
                soundID, emitterPos, volume, maxDistance, _fxLinkedSoundList, ref _fxVoiceSendDescriptor, onFXEnd);
        }

        /// <summary>
        ///     play a fx sound
        /// </summary>
        /// <param name="soundID">the sound id</param>
        /// <param name="emitter">the emitter</param>
        /// <param name="volume">volume</param>
        /// <param name="onFXEnd">called than the sound ends</param>
        public void PlayFxSound(int soundID, Emitter emitter, float volume, Action<IntPtr> onFXEnd = null)
        {
            if (_fxLinkedSoundList.Count >= _fxLinkedSoundList.Capacity)
            {
                return;
            }
            PlaySound(soundID, emitter, volume, _fxLinkedSoundList, ref _fxVoiceSendDescriptor, onFXEnd);
        }

        /// <summary>
        ///     pause all fx sounds
        /// </summary>
        public void PauseFxSounds()
        {
            foreach (Sound sound in _fxLinkedSoundList.Enumerate())
            {
                sound.SourceVoice.Stop();
            }
        }

        /// <summary>
        ///     resume all fx sounds
        /// </summary>
        public void ResumeFxSounds()
        {
            foreach (Sound sound in _fxLinkedSoundList.Enumerate())
            {
                sound.SourceVoice.Start();
            }
        }

        /// <summary>
        ///     stops all fx sounds
        /// </summary>
        public void StopFxSounds()
        {
            foreach (Sound sound in _fxLinkedSoundList.Enumerate())
            {
                sound.SourceVoice.Stop();
                sound.SourceVoice.DestroyVoice();
                sound.SourceVoice.Dispose();
            }
        }

        /// <summary>
        ///     play an environment sound
        /// </summary>
        /// <param name="soundID">the sound id</param>
        /// <param name="emitterPos">emitter position</param>
        /// <param name="volume">volume</param>
        /// <param name="maxDistance">max distance to hear the sound</param>
        /// <param name="onFXEnd">called than the sound ends</param>
        public void PlayEnvSound(int soundID, Vector3 emitterPos, float volume, float maxDistance,
            Action<IntPtr> onFXEnd = null)
        {
            PlaySound(
                soundID, emitterPos, volume, maxDistance, _envLinkedSoundList, ref _envVoiceSendDescriptor, onFXEnd);
        }

        /// <summary>
        ///     play an environment sound
        /// </summary>
        /// <param name="soundID">the sound id</param>
        /// <param name="emitter">the emitter</param>
        /// <param name="volume">volume</param>
        /// <param name="onFXEnd">called than the sound ends</param>
        public void PlayEnvSound(int soundID, Emitter emitter, float volume, Action<IntPtr> onFXEnd = null)
        {
            PlaySound(soundID, emitter, volume, _envLinkedSoundList, ref _envVoiceSendDescriptor, onFXEnd);
        }

        /// <summary>
        ///     pause all environment sounds
        /// </summary>
        public void PauseEnvSounds()
        {
            foreach (Sound sound in _envLinkedSoundList.Enumerate())
            {
                sound.SourceVoice.Stop();
            }
        }

        /// <summary>
        ///     resume all environment sounds
        /// </summary>
        public void ResumeEnvSounds()
        {
            foreach (Sound sound in _envLinkedSoundList.Enumerate())
            {
                sound.SourceVoice.Start();
            }
        }

        /// <summary>
        ///     stops all environment sounds
        /// </summary>
        public void StopEnvSounds()
        {
            foreach (Sound sound in _envLinkedSoundList.Enumerate())
            {
                sound.SourceVoice.Stop();
                sound.SourceVoice.DestroyVoice();
                sound.SourceVoice.Dispose();
            }
        }

        private void PlaySound(int soundID, Vector3 emitterPos, float volume, float maxDistanance, LinkedSoundList list,
            ref VoiceSendDescriptor voiceSendDescriptor, Action<IntPtr> onFXEnd = null)
        {
            PlaySound(
                soundID, new Emitter
                {
                    ChannelCount = 1,
                    VolumeCurve = new[]
                    {
                        new CurvePoint { Distance = 0.0f, DspSetting = 1.0f },
                        new CurvePoint { Distance = 1.0f, DspSetting = 0.0f }
                    },
                    CurveDistanceScaler = maxDistanance,
                    OrientFront = Vector3.UnitZ,
                    OrientTop = Vector3.UnitY,
                    Position = emitterPos,
                    Velocity = new Vector3(0, 0, 0)
                }, volume, list, ref voiceSendDescriptor, onFXEnd);
        }

        private void PlaySound(int soundID, Emitter emitter, float volume, LinkedSoundList list,
            ref VoiceSendDescriptor voiceSendDescriptor, Action<IntPtr> onFXEnd = null)
        {
            if (!_soundBuffer.TryGetValue(soundID, out SoundBuffer buffer))
            {
                return;
            }

            SourceVoice sourceVoice = new SourceVoice(_xAudio2, buffer.Format, VoiceFlags.None, true);
            sourceVoice.SetVolume(volume);
            sourceVoice.SubmitSourceBuffer(buffer.AudioBuffer, buffer.DecodedPacketsInfo);
            sourceVoice.SetOutputVoices(voiceSendDescriptor);

            Sound sound = new Sound
            {
                SourceVoice = sourceVoice,
                Emitter = emitter
            };

            list.Add(sound);

            sourceVoice.BufferEnd += _ =>
            {
                list.Remove(sound);
                sourceVoice.DestroyVoice();
            };

            if (onFXEnd != null)
            {
                sourceVoice.BufferEnd += onFXEnd;
            }
            sourceVoice.Start();

            DspSettings settings = _x3dAudio.Calculate(
                _listener,
                sound.Emitter,
                CalculateFlags.Matrix | CalculateFlags.Doppler,
                buffer.Format.Channels,
                _inputChannelCount);
            sound.SourceVoice.SetOutputMatrix(buffer.Format.Channels, _inputChannelCount, settings.MatrixCoefficients);
            sound.SourceVoice.SetFrequencyRatio(settings.DopplerFactor);
        }

        private void RecalculateFXSounds()
        {
            DspSettings settings = null;
            foreach (Sound sound in _fxLinkedSoundList.Enumerate())
            {
                settings = _x3dAudio.Calculate(
                    _listener,
                    sound.Emitter,
                    CalculateFlags.Matrix | CalculateFlags.Doppler,
                    sound.SourceVoice.VoiceDetails.InputChannelCount,
                    _inputChannelCount);
                sound.SourceVoice.SetOutputMatrix(
                    sound.SourceVoice.VoiceDetails.InputChannelCount, _inputChannelCount, settings.MatrixCoefficients);
                sound.SourceVoice.SetFrequencyRatio(settings.DopplerFactor);
            }
        }

        private void RecalculateEnvSounds()
        {
            DspSettings settings = null;
            foreach (Sound sound in _envLinkedSoundList.Enumerate())
            {
                settings = _x3dAudio.Calculate(
                    _listener,
                    sound.Emitter,
                    CalculateFlags.Matrix | CalculateFlags.Doppler,
                    sound.SourceVoice.VoiceDetails.InputChannelCount,
                    _inputChannelCount);
                sound.SourceVoice.SetOutputMatrix(
                    sound.SourceVoice.VoiceDetails.InputChannelCount, _inputChannelCount, settings.MatrixCoefficients);
                sound.SourceVoice.SetFrequencyRatio(settings.DopplerFactor);
            }
        }

        /// <summary>
        ///     starts a back ground music song
        /// </summary>
        /// <param name="songID">the song id</param>
        /// <param name="onBGMEnd">called than the song ends</param>
        public void RunBGM(int songID, Action<IntPtr> onBGMEnd = null)
        {
            if (!_soundBuffer.TryGetValue(songID, out SoundBuffer buffer))
            {
                return;
            }
            if (_currentBGM?.State.BuffersQueued > 0)
            {
                _currentBGM.Stop();
            }

            _currentBGM = new SourceVoice(_xAudio2, buffer.Format, VoiceFlags.None, true);
            _currentBGM.SetVolume(_bgmVolume);
            _currentBGM.SubmitSourceBuffer(buffer.AudioBuffer, buffer.DecodedPacketsInfo);

            _currentBGM.BufferEnd += ptr =>
            {
                _currentBGM.DestroyVoice();
            };
            if (onBGMEnd != null)
            {
                _currentBGM.BufferEnd += onBGMEnd;
            }

            _currentBGM.SubmitSourceBuffer(buffer.AudioBuffer, buffer.DecodedPacketsInfo);
            _currentBGM.Start();
        }

        /// <summary>
        ///     resume a bgm song
        /// </summary>
        public void ResumeBGM()
        {
            if (_currentBGM?.State.BuffersQueued > 0)
            {
                _currentBGM.Start();
            }
        }

        /// <summary>
        ///     pause a bgm song
        /// </summary>
        public void PauseBGM()
        {
            if (_currentBGM?.State.BuffersQueued > 0)
            {
                _currentBGM.Stop();
            }
        }

        /// <summary>
        ///     stop a bgm song
        /// </summary>
        public void StopBGM()
        {
            if (_currentBGM?.State.BuffersQueued > 0)
            {
                _currentBGM.Stop();
                _currentBGM.DestroyVoice();
                _currentBGM.Dispose();
            }
        }

        #region IDisposable Support

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    /* USER CODE */
                    StopFxSounds();
                    StopEnvSounds();

                    _currentBGM?.DestroyVoice();
                    _currentBGM?.Dispose();
                    _currentBGM = null;

                    _fxLinkedSoundList?.Clear();
                    _fxLinkedSoundList = null;

                    _envLinkedSoundList?.Clear();
                    _envLinkedSoundList = null;

                    UnloadAll();
                    _soundBuffer = null;

                    _fxSubmixVoice?.DestroyVoice();
                    _fxSubmixVoice?.Dispose();
                    _fxSubmixVoice = null;

                    _envSubmixVoice?.DestroyVoice();
                    _envSubmixVoice?.Dispose();
                    _envSubmixVoice = null;

                    _masteringVoice?.DestroyVoice();
                    _masteringVoice?.Dispose();
                    _masteringVoice = null;

                    _xAudio2?.Dispose();
                    _xAudio2 = null;
                    _x3dAudio = null;
                }

                _disposed = true;
            }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged/managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    #endregion
}