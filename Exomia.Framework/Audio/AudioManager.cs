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
using System.Collections.Generic;
using System.IO;
using SharpDX;
using SharpDX.Multimedia;
using SharpDX.X3DAudio;
using SharpDX.XAudio2;

namespace Exomia.Framework.Audio
{
    /// <inheritdoc />
    /// <summary>
    ///     AudioManager class
    /// </summary>
    public sealed class AudioManager : IDisposable
    {
        private readonly int _inputChannelCount;

        private readonly Listener _listener;
        private float _bgmVolume = 1.0f;

        private SourceVoice _currentBgm;
        private LinkedSoundList _envLinkedSoundList;
        private SubmixVoice _envSubmixVoice;
        private VoiceSendDescriptor _envVoiceSendDescriptor;
        private float _envVolume = 1.0f;

        private LinkedSoundList _fxLinkedSoundList;
        private SubmixVoice _fxSubmixVoice;

        private VoiceSendDescriptor _fxVoiceSendDescriptor;
        private float _fxVolume = 1.0f;
        private MasteringVoice _masteringVoice;

        private float _masterVolume = 0.5f;

        private Dictionary<int, SoundBuffer> _soundBuffer;
        private int _soundBufferIndex;
        private X3DAudio _x3DAudio;

        private XAudio2 _xAudio2;

        /// <summary>
        ///     BGMVolume
        /// </summary>
        public float BgmVolume
        {
            get { return _bgmVolume; }
            set
            {
                _bgmVolume = MathUtil.Clamp(value, 0.0f, 1.0f);
                if (_currentBgm != null && _currentBgm.State.BuffersQueued > 0)
                {
                    _currentBgm.SetVolume(_bgmVolume);
                }
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

        /// <summary>
        ///     FXVolume
        /// </summary>
        public float FxVolume
        {
            get { return _fxVolume; }
            set
            {
                _fxVolume = MathUtil.Clamp(value, 0.0f, 1.0f);
                _fxSubmixVoice?.SetVolume(_fxVolume);
            }
        }

        /// <summary>
        ///     ListenerPosition
        /// </summary>
        public Vector3 ListenerPosition
        {
            get { return _listener.Position; }
            set
            {
                _listener.Position = value;
                RecalculateFxSounds();
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
                RecalculateFxSounds();
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
            _soundBuffer        = new Dictionary<int, SoundBuffer>(128);
            _fxLinkedSoundList  = new LinkedSoundList(fxSoundPoolLimit);
            _envLinkedSoundList = new LinkedSoundList();

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
            _x3DAudio = new X3DAudio(speakers, X3DAudio.SpeedOfSound / 1000f, X3DAudioVersion.Default);

            _masteringVoice.GetVoiceDetails(out VoiceDetails details);
            _inputChannelCount = details.InputChannelCount;

            _fxSubmixVoice = new SubmixVoice(_xAudio2, _inputChannelCount, details.InputSampleRate);
            _fxSubmixVoice.SetVolume(_fxVolume);

            _envSubmixVoice = new SubmixVoice(_xAudio2, _inputChannelCount, details.InputSampleRate);
            _envSubmixVoice.SetVolume(_envVolume);

            _fxVoiceSendDescriptor  = new VoiceSendDescriptor(VoiceSendFlags.None, _fxSubmixVoice);
            _envVoiceSendDescriptor = new VoiceSendDescriptor(VoiceSendFlags.None, _envSubmixVoice);
        }

        /// <summary>
        ///     destructor
        /// </summary>
        ~AudioManager()
        {
            Dispose(false);
        }

        /// <summary>
        ///     load a sound from a given stream and returns the sound id
        /// </summary>
        /// <param name="stream">sound stream to load the sound from</param>
        /// <returns>sound id</returns>
        public int LoadSound(Stream stream)
        {
            using (SoundStream soundStream = new SoundStream(stream))
            {
                AudioBuffer audioBuffer = new AudioBuffer
                {
                    Stream     = soundStream.ToDataStream(),
                    AudioBytes = (int)soundStream.Length,
                    Flags      = BufferFlags.EndOfStream
                };
                soundStream.Close();
                _soundBuffer.Add(
                    _soundBufferIndex,
                    new SoundBuffer
                    {
                        AudioBuffer        = audioBuffer,
                        Format             = soundStream.Format,
                        DecodedPacketsInfo = soundStream.DecodedPacketsInfo
                    });
            }
            return _soundBufferIndex++;
        }

        /// <summary>
        ///     pause a bgm song
        /// </summary>
        public void PauseBgm()
        {
            if (_currentBgm?.State.BuffersQueued > 0)
            {
                _currentBgm.Stop();
            }
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
        ///     play an environment sound
        /// </summary>
        /// <param name="soundID">the sound id</param>
        /// <param name="emitterPos">emitter position</param>
        /// <param name="volume">volume</param>
        /// <param name="maxDistance">max distance to hear the sound</param>
        /// <param name="onFxEnd">called than the sound ends</param>
        public void PlayEnvSound(int soundID, Vector3 emitterPos, float volume, float maxDistance,
            Action<IntPtr> onFxEnd = null)
        {
            PlaySound(
                soundID, emitterPos, volume, maxDistance, _envLinkedSoundList, ref _envVoiceSendDescriptor, onFxEnd);
        }

        /// <summary>
        ///     play an environment sound
        /// </summary>
        /// <param name="soundID">the sound id</param>
        /// <param name="emitter">the emitter</param>
        /// <param name="volume">volume</param>
        /// <param name="onFxEnd">called than the sound ends</param>
        public void PlayEnvSound(int soundID, Emitter emitter, float volume, Action<IntPtr> onFxEnd = null)
        {
            PlaySound(soundID, emitter, volume, _envLinkedSoundList, ref _envVoiceSendDescriptor, onFxEnd);
        }

        /// <summary>
        ///     play a fx sound
        /// </summary>
        /// <param name="soundID">the sound id</param>
        /// <param name="emitterPos">emitter position</param>
        /// <param name="volume">volume</param>
        /// <param name="maxDistance">max distance to hear the sound</param>
        /// <param name="onFxEnd">called than the sound ends</param>
        public void PlayFxSound(int soundID, Vector3 emitterPos, float volume, float maxDistance,
            Action<IntPtr> onFxEnd = null)
        {
            if (_fxLinkedSoundList.Count >= _fxLinkedSoundList.Capacity)
            {
                return;
            }
            PlaySound(
                soundID, emitterPos, volume, maxDistance, _fxLinkedSoundList, ref _fxVoiceSendDescriptor, onFxEnd);
        }

        /// <summary>
        ///     play a fx sound
        /// </summary>
        /// <param name="soundID">the sound id</param>
        /// <param name="emitter">the emitter</param>
        /// <param name="volume">volume</param>
        /// <param name="onFxEnd">called than the sound ends</param>
        public void PlayFxSound(int soundID, Emitter emitter, float volume, Action<IntPtr> onFxEnd = null)
        {
            if (_fxLinkedSoundList.Count >= _fxLinkedSoundList.Capacity)
            {
                return;
            }
            PlaySound(soundID, emitter, volume, _fxLinkedSoundList, ref _fxVoiceSendDescriptor, onFxEnd);
        }

        /// <summary>
        ///     resume a bgm song
        /// </summary>
        public void ResumeBgm()
        {
            if (_currentBgm?.State.BuffersQueued > 0)
            {
                _currentBgm.Start();
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
        ///     starts a back ground music song
        /// </summary>
        /// <param name="songID">the song id</param>
        /// <param name="onBgmEnd">called than the song ends</param>
        public void RunBgm(int songID, Action<IntPtr> onBgmEnd = null)
        {
            if (!_soundBuffer.TryGetValue(songID, out SoundBuffer buffer))
            {
                return;
            }
            if (_currentBgm?.State.BuffersQueued > 0)
            {
                _currentBgm.Stop();
            }

            _currentBgm = new SourceVoice(_xAudio2, buffer.Format, VoiceFlags.None, true);
            _currentBgm.SetVolume(_bgmVolume);
            _currentBgm.SubmitSourceBuffer(buffer.AudioBuffer, buffer.DecodedPacketsInfo);

            _currentBgm.BufferEnd += ptr =>
            {
                _currentBgm.DestroyVoice();
            };
            if (onBgmEnd != null)
            {
                _currentBgm.BufferEnd += onBgmEnd;
            }

            _currentBgm.SubmitSourceBuffer(buffer.AudioBuffer, buffer.DecodedPacketsInfo);
            _currentBgm.Start();
        }

        /// <summary>
        ///     stop a bgm song
        /// </summary>
        public void StopBgm()
        {
            if (_currentBgm?.State.BuffersQueued > 0)
            {
                _currentBgm.Stop();
                _currentBgm.DestroyVoice();
                _currentBgm.Dispose();
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
        ///     unload all loaded sounds and free resources
        /// </summary>
        public void UnloadAll()
        {
            StopBgm();
            _soundBuffer.Clear();
            _soundBufferIndex = 0;
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

        private void PlaySound(int soundID, Vector3 emitterPos, float volume, float maxDistance, LinkedSoundList list,
            ref VoiceSendDescriptor voiceSendDescriptor, Action<IntPtr> onFxEnd = null)
        {
            PlaySound(
                soundID,
                new Emitter
                {
                    ChannelCount = 1,
                    VolumeCurve =
                        new[]
                        {
                            new CurvePoint { Distance = 0.0f, DspSetting = 1.0f },
                            new CurvePoint { Distance = 1.0f, DspSetting = 0.0f }
                        },
                    CurveDistanceScaler = maxDistance,
                    OrientFront         = Vector3.UnitZ,
                    OrientTop           = Vector3.UnitY,
                    Position            = emitterPos,
                    Velocity            = new Vector3(0, 0, 0)
                }, volume, list, ref voiceSendDescriptor, onFxEnd);
        }

        private void PlaySound(int soundID, Emitter emitter, float volume, LinkedSoundList list,
            ref VoiceSendDescriptor voiceSendDescriptor, Action<IntPtr> onFxEnd = null)
        {
            if (!_soundBuffer.TryGetValue(soundID, out SoundBuffer buffer))
            {
                return;
            }

            SourceVoice sourceVoice = new SourceVoice(_xAudio2, buffer.Format, VoiceFlags.None, true);
            sourceVoice.SetVolume(volume);
            sourceVoice.SubmitSourceBuffer(buffer.AudioBuffer, buffer.DecodedPacketsInfo);
            sourceVoice.SetOutputVoices(voiceSendDescriptor);

            Sound sound = new Sound(emitter, sourceVoice);
            list.Add(sound);

            sourceVoice.BufferEnd += _ =>
            {
                list.Remove(sound);
                sourceVoice.DestroyVoice();
            };

            if (onFxEnd != null)
            {
                sourceVoice.BufferEnd += onFxEnd;
            }
            sourceVoice.Start();

            DspSettings settings = _x3DAudio.Calculate(
                _listener,
                sound.Emitter,
                CalculateFlags.Matrix | CalculateFlags.Doppler,
                buffer.Format.Channels,
                _inputChannelCount);
            sound.SourceVoice.SetOutputMatrix(buffer.Format.Channels, _inputChannelCount, settings.MatrixCoefficients);
            sound.SourceVoice.SetFrequencyRatio(settings.DopplerFactor);
        }

        private void RecalculateEnvSounds()
        {
            foreach (Sound sound in _envLinkedSoundList.Enumerate())
            {
                DspSettings settings = _x3DAudio.Calculate(
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

        private void RecalculateFxSounds()
        {
            foreach (Sound sound in _fxLinkedSoundList.Enumerate())
            {
                DspSettings settings = _x3DAudio.Calculate(
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

        private struct SoundBuffer
        {
            public AudioBuffer AudioBuffer;
            public WaveFormat Format;
            public uint[] DecodedPacketsInfo;
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

                    _currentBgm?.DestroyVoice();
                    _currentBgm?.Dispose();
                    _currentBgm = null;

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
                    _xAudio2  = null;
                    _x3DAudio = null;
                }

                _disposed = true;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}