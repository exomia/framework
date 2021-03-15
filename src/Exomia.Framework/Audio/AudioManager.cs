#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Exomia.Framework.Audio
{
    // TODO: 
    #if AUDIO_SUPPORT
    /// <summary>
    ///     Manager for audio. This class cannot be inherited.
    /// </summary>
    public sealed class AudioManager : IAudioManager
    {
        private readonly int _inputChannelCount;
        private readonly Listener _listener;
        private readonly LinkedSoundList _envLinkedSoundList;
        private readonly SubmixVoice _envSubmixVoice;
        private readonly LinkedSoundList _fxLinkedSoundList;
        private readonly SubmixVoice _fxSubmixVoice;
        private readonly MasteringVoice _masteringVoice;
        private readonly Dictionary<int, SoundBuffer> _soundBuffer;
        private readonly X3DAudio _x3DAudio;
        private readonly XAudio2 _xAudio2;
        private float _bgmVolume = 1.0f;
        private SourceVoice? _currentBgm;
        private VoiceSendDescriptor _envVoiceSendDescriptor;
        private float _envVolume = 1.0f;
        private VoiceSendDescriptor _fxVoiceSendDescriptor;
        private float _fxVolume = 1.0f;
        private float _masterVolume = 0.5f;
        private int _soundBufferIndex;

        /// <inheritdoc />
        public float BgmVolume
        {
            get { return _bgmVolume; }
            set
            {
                _bgmVolume = Math2.Clamp(value, 0.0f, 1.0f);
                if (_currentBgm != null && _currentBgm.State.BuffersQueued > 0)
                {
                    _currentBgm.SetVolume(_bgmVolume);
                }
            }
        }

        /// <inheritdoc />
        public float EnvironmentVolume
        {
            get { return _envVolume; }
            set
            {
                _envVolume = Math2.Clamp(value, 0.0f, 1.0f);
                _envSubmixVoice.SetVolume(_envVolume);
            }
        }

        /// <inheritdoc />
        public float FxVolume
        {
            get { return _fxVolume; }
            set
            {
                _fxVolume = Math2.Clamp(value, 0.0f, 1.0f);
                _fxSubmixVoice.SetVolume(_fxVolume);
            }
        }

        /// <inheritdoc />
        public float MasterVolume
        {
            get { return _masterVolume; }
            set
            {
                _masterVolume = Math2.Clamp(value, 0.0f, 1.0f);
                _masteringVoice.SetVolume(_masterVolume);
            }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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
        ///     Initializes a new instance of the <see cref="AudioManager" /> class.
        /// </summary>
        /// <param name="listener">         The listener. </param>
        /// <param name="fxSoundPoolLimit"> The effects sound pool limit. </param>
        /// <param name="speakers">         The speakers. </param>
        /// <param name="deviceID">         (Optional) Identifier for the device. </param>
        /// <exception cref="ArgumentException">
        ///     Thrown when one or more arguments have unsupported or
        ///     illegal values.
        /// </exception>
        public AudioManager(Listener listener, int fxSoundPoolLimit, Speakers speakers, string? deviceID = null)
        {
            _listener = listener;
            if (fxSoundPoolLimit <= 0)
            {
                throw new ArgumentException("fxSoundPoolLimit must be bigger than 0");
            }
            _soundBuffer = new Dictionary<int, SoundBuffer>(128);
            _fxLinkedSoundList = new LinkedSoundList(fxSoundPoolLimit);
            _envLinkedSoundList = new LinkedSoundList();

#if DEBUG
            _xAudio2 = new XAudio2(XAudio2Flags.DebugEngine, ProcessorSpecifier.AnyProcessor);
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
                else
                {
                    _masteringVoice = new MasteringVoice(
                        _xAudio2, XAudio2.DefaultChannels, XAudio2.DefaultSampleRate, deviceID);
                }
            }
            else
            {
                _masteringVoice = new MasteringVoice(
                    _xAudio2, XAudio2.DefaultChannels, XAudio2.DefaultSampleRate, deviceID);
            }
            _masteringVoice.SetVolume(_masterVolume);

            _x3DAudio = new X3DAudio(speakers, X3DAudio.SpeedOfSound);

            _masteringVoice.GetVoiceDetails(out VoiceDetails details);
            _inputChannelCount = details.InputChannelCount;

            _fxSubmixVoice = new SubmixVoice(_xAudio2, _inputChannelCount, details.InputSampleRate);
            _fxSubmixVoice.SetVolume(_fxVolume);

            _envSubmixVoice = new SubmixVoice(_xAudio2, _inputChannelCount, details.InputSampleRate);
            _envSubmixVoice.SetVolume(_envVolume);

            _fxVoiceSendDescriptor = new VoiceSendDescriptor(VoiceSendFlags.None, _fxSubmixVoice);
            _envVoiceSendDescriptor = new VoiceSendDescriptor(VoiceSendFlags.None, _envSubmixVoice);
        }

        /// <inheritdoc />
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
                    _soundBufferIndex,
                    new SoundBuffer
                    {
                        AudioBuffer = audioBuffer,
                        Format = soundStream.Format,
                        DecodedPacketsInfo = soundStream.DecodedPacketsInfo
                    });
            }
            return _soundBufferIndex++;
        }

        /// <inheritdoc />
        public void PauseBgm()
        {
            if (_currentBgm?.State.BuffersQueued > 0)
            {
                _currentBgm.Stop();
            }
        }

        /// <inheritdoc />
        public void PauseEnvSounds()
        {
            foreach (LinkedSoundList.Sound sound in _envLinkedSoundList)
            {
                sound.SourceVoice.Stop();
            }
        }

        /// <inheritdoc />
        public void PauseFxSounds()
        {
            foreach (LinkedSoundList.Sound sound in _fxLinkedSoundList)
            {
                sound.SourceVoice.Stop();
            }
        }

        /// <inheritdoc />
        public void PlayEnvSound(int soundID,
                                 Vector3 emitterPos,
                                 float volume,
                                 float maxDistance,
                                 Action<IntPtr>? onFxEnd = null)
        {
            PlaySound(
                soundID, emitterPos, volume, maxDistance, _envLinkedSoundList, ref _envVoiceSendDescriptor, onFxEnd);
        }

        /// <inheritdoc />
        public void PlayFxSound(int soundID,
                                Vector3 emitterPos,
                                float volume,
                                float maxDistance,
                                Action<IntPtr>? onFxEnd = null)
        {
            if (_fxLinkedSoundList.Count >= _fxLinkedSoundList.Capacity)
            {
                return;
            }
            PlaySound(
                soundID, emitterPos, volume, maxDistance, _fxLinkedSoundList, ref _fxVoiceSendDescriptor, onFxEnd);
        }

        /// <inheritdoc />
        public void PlayEnvSound(int soundID, Emitter emitter, float volume, Action<IntPtr>? onFxEnd = null)
        {
            PlaySound(soundID, emitter, volume, _envLinkedSoundList, ref _envVoiceSendDescriptor, onFxEnd);
        }

        /// <inheritdoc />
        public void PlayFxSound(int soundID, Emitter emitter, float volume, Action<IntPtr>? onFxEnd = null)
        {
            if (_fxLinkedSoundList.Count >= _fxLinkedSoundList.Capacity)
            {
                return;
            }
            PlaySound(soundID, emitter, volume, _fxLinkedSoundList, ref _fxVoiceSendDescriptor, onFxEnd);
        }

        /// <inheritdoc />
        public void ResumeBgm()
        {
            if (_currentBgm?.State.BuffersQueued > 0)
            {
                _currentBgm.Start();
            }
        }

        /// <inheritdoc />
        public void ResumeEnvSounds()
        {
            foreach (LinkedSoundList.Sound sound in _envLinkedSoundList)
            {
                sound.SourceVoice.Start();
            }
        }

        /// <inheritdoc />
        public void ResumeFxSounds()
        {
            foreach (LinkedSoundList.Sound sound in _fxLinkedSoundList)
            {
                sound.SourceVoice.Start();
            }
        }

        /// <inheritdoc />
        public void RunBgm(int songID, Action<IntPtr>? onBgmEnd = null)
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

        /// <inheritdoc />
        public void StopBgm()
        {
            if (_currentBgm?.State.BuffersQueued > 0)
            {
                _currentBgm.Stop();
                _currentBgm.DestroyVoice();
                _currentBgm.Dispose();
            }
        }

        /// <inheritdoc />
        public void StopEnvSounds()
        {
            foreach (LinkedSoundList.Sound sound in _envLinkedSoundList)
            {
                sound.SourceVoice.Stop();
                sound.SourceVoice.DestroyVoice();
                sound.SourceVoice.Dispose();
            }
        }

        /// <inheritdoc />
        public void StopFxSounds()
        {
            foreach (LinkedSoundList.Sound sound in _fxLinkedSoundList)
            {
                sound.SourceVoice.Stop();
                sound.SourceVoice.DestroyVoice();
                sound.SourceVoice.Dispose();
            }
        }

        /// <inheritdoc />
        public void UnloadAll()
        {
            StopBgm();
            _soundBuffer.Clear();
            _soundBufferIndex = 0;
        }

        /// <inheritdoc />
        public bool UnloadSound(int soundID)
        {
            return _soundBuffer.Remove(soundID);
        }

        private void PlaySound(int soundID,
                               Vector3 emitterPos,
                               float volume,
                               float maxDistance,
                               LinkedSoundList list,
                               ref VoiceSendDescriptor voiceSendDescriptor,
                               Action<IntPtr>? onFxEnd = null)
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
                    OrientFront = Vector3.UnitZ,
                    OrientTop = Vector3.UnitY,
                    Position = emitterPos,
                    Velocity = new Vector3(0, 0, 0)
                }, volume, list, ref voiceSendDescriptor, onFxEnd);
        }

        private void PlaySound(int soundID,
                               Emitter emitter,
                               float volume,
                               LinkedSoundList list,
                               ref VoiceSendDescriptor voiceSendDescriptor,
                               Action<IntPtr>? onFxEnd = null)
        {
            if (!_soundBuffer.TryGetValue(soundID, out SoundBuffer buffer))
            {
                return;
            }

            SourceVoice sourceVoice = new SourceVoice(_xAudio2, buffer.Format, VoiceFlags.None, true);
            sourceVoice.SetVolume(volume);
            sourceVoice.SubmitSourceBuffer(buffer.AudioBuffer, buffer.DecodedPacketsInfo);
            sourceVoice.SetOutputVoices(voiceSendDescriptor);

            LinkedSoundList.Sound sound = new LinkedSoundList.Sound(emitter, sourceVoice);
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
            foreach (LinkedSoundList.Sound sound in _envLinkedSoundList)
            {
                DspSettings settings = _x3DAudio.Calculate(
                    _listener,
                    sound.Emitter,
                    CalculateFlags.Matrix | CalculateFlags.Doppler,
                    sound.SourceVoice.VoiceDetails.InputChannelCount,
                    _inputChannelCount);
                sound.SourceVoice.SetOutputMatrix(
                    sound.SourceVoice.VoiceDetails.InputChannelCount, _inputChannelCount,
                    settings.MatrixCoefficients);
                sound.SourceVoice.SetFrequencyRatio(settings.DopplerFactor);
            }
        }

        private void RecalculateFxSounds()
        {
            foreach (LinkedSoundList.Sound sound in _fxLinkedSoundList)
            {
                DspSettings settings = _x3DAudio.Calculate(
                    _listener,
                    sound.Emitter,
                    CalculateFlags.Matrix | CalculateFlags.Doppler,
                    sound.SourceVoice.VoiceDetails.InputChannelCount,
                    _inputChannelCount);
                sound.SourceVoice.SetOutputMatrix(
                    sound.SourceVoice.VoiceDetails.InputChannelCount, _inputChannelCount,
                    settings.MatrixCoefficients);
                sound.SourceVoice.SetFrequencyRatio(settings.DopplerFactor);
            }
        }

        private struct SoundBuffer
        {
            /// <summary>
            ///     Buffer for audio data.
            /// </summary>
            public AudioBuffer AudioBuffer;

            /// <summary>
            ///     Describes the format to use.
            /// </summary>
            public WaveFormat Format;

            /// <summary>
            ///     Information describing the decoded packets.
            /// </summary>
            public uint[] DecodedPacketsInfo;
        }

        #region IDisposable Support

        /// <summary>
        ///     True if disposed.
        /// </summary>
        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    StopFxSounds();
                    StopEnvSounds();

                    _currentBgm?.DestroyVoice();
                    _currentBgm?.Dispose();
                    _currentBgm = null;

                    _fxLinkedSoundList.Clear();
                    _envLinkedSoundList.Clear();

                    UnloadAll();

                    _fxSubmixVoice.DestroyVoice();
                    _fxSubmixVoice.Dispose();

                    _envSubmixVoice.DestroyVoice();
                    _envSubmixVoice.Dispose();

                    _masteringVoice.DestroyVoice();
                    _masteringVoice.Dispose();

                    _xAudio2.Dispose();
                }

                _disposed = true;
            }
        }

        /// <inheritdoc />
        ~AudioManager()
        {
            Dispose(false);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
    #endif
}