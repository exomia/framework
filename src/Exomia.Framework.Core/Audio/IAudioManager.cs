#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.IO;
using System.Numerics;

namespace Exomia.Framework.Core.Audio
{
    /// <summary>
    ///     Interface for audio manager.
    /// </summary>
    public interface IAudioManager : IDisposable
    {
        /// <summary>
        ///     BGMVolume.
        /// </summary>
        /// <value>
        ///     The bgm volume.
        /// </value>
        float BgmVolume { get; set; }

        /// <summary>
        ///     EnvironmentVolume.
        /// </summary>
        /// <value>
        ///     The environment volume.
        /// </value>
        float EnvironmentVolume { get; set; }

        /// <summary>
        ///     FxVolume.
        /// </summary>
        /// <value>
        ///     The effects volume.
        /// </value>
        float FxVolume { get; set; }

        /// <summary>
        ///     MasterVolume.
        /// </summary>
        /// <value>
        ///     The master volume.
        /// </value>
        float MasterVolume { get; set; }

        /// <summary>
        ///     ListenerPosition.
        /// </summary>
        /// <value>
        ///     The listener position.
        /// </value>
        Vector3 ListenerPosition { get; set; }

        /// <summary>
        ///     ListenerVelocity.
        /// </summary>
        /// <value>
        ///     The listener velocity.
        /// </value>
        Vector3 ListenerVelocity { get; set; }

        /// <summary>
        ///     load a sound from a given stream and returns the sound id.
        /// </summary>
        /// <param name="stream"> sound stream to load the sound from. </param>
        /// <returns>
        ///     sound id.
        /// </returns>
        int LoadSound(Stream stream);

        /// <summary>
        ///     pause a bgm song.
        /// </summary>
        void PauseBgm();

        /// <summary>
        ///     pause all environment sounds.
        /// </summary>
        void PauseEnvSounds();

        /// <summary>
        ///     pause all fx sounds.
        /// </summary>
        void PauseFxSounds();

        /// <summary>
        ///     play an environment sound.
        /// </summary>
        /// <param name="soundID">     the sound id. </param>
        /// <param name="emitterPos">  emitter position. </param>
        /// <param name="volume">      volume. </param>
        /// <param name="maxDistance"> max distance to hear the sound. </param>
        /// <param name="onFxEnd">     (Optional) called than the sound ends. </param>
        void PlayEnvSound(int             soundID,
                          Vector3         emitterPos,
                          float           volume,
                          float           maxDistance,
                          Action<IntPtr>? onFxEnd = null);

        /// <summary>
        ///     play a fx sound.
        /// </summary>
        /// <param name="soundID">     the sound id. </param>
        /// <param name="emitterPos">  emitter position. </param>
        /// <param name="volume">      volume. </param>
        /// <param name="maxDistance"> max distance to hear the sound. </param>
        /// <param name="onFxEnd">     (Optional) called than the sound ends. </param>
        void PlayFxSound(int             soundID,
                         Vector3         emitterPos,
                         float           volume,
                         float           maxDistance,
                         Action<IntPtr>? onFxEnd = null);

        /// <summary>
        ///     play an environment sound.
        /// </summary>
        /// <param name="soundID"> the sound id. </param>
        /// <param name="emitter"> the emitter. </param>
        /// <param name="volume">  volume. </param>
        /// <param name="onFxEnd"> (Optional) called than the sound ends. </param>
        void PlayEnvSound(int soundID, Emitter emitter, float volume, Action<IntPtr>? onFxEnd = null);

        /// <summary>
        ///     play a fx sound.
        /// </summary>
        /// <param name="soundID"> the sound id. </param>
        /// <param name="emitter"> the emitter. </param>
        /// <param name="volume">  volume. </param>
        /// <param name="onFxEnd"> (Optional) called than the sound ends. </param>
        void PlayFxSound(int soundID, Emitter emitter, float volume, Action<IntPtr>? onFxEnd = null);

        /// <summary>
        ///     resume a bgm song.
        /// </summary>
        void ResumeBgm();

        /// <summary>
        ///     resume all environment sounds.
        /// </summary>
        void ResumeEnvSounds();

        /// <summary>
        ///     resume all fx sounds.
        /// </summary>
        void ResumeFxSounds();

        /// <summary>
        ///     starts a back ground music song.
        /// </summary>
        /// <param name="songID">   the song id. </param>
        /// <param name="onBgmEnd"> (Optional) called than the song ends. </param>
        void RunBgm(int songID, Action<IntPtr>? onBgmEnd = null);

        /// <summary>
        ///     stop a bgm song.
        /// </summary>
        void StopBgm();

        /// <summary>
        ///     stops all environment sounds.
        /// </summary>
        void StopEnvSounds();

        /// <summary>
        ///     stops all fx sounds.
        /// </summary>
        void StopFxSounds();

        /// <summary>
        ///     unload all loaded sounds and free resources.
        /// </summary>
        void UnloadAll();

        /// <summary>
        ///     unload a sound and free resources from a given sound id.
        /// </summary>
        /// <param name="soundID"> the sound id. </param>
        /// <returns>
        ///     <c>true</c>if sound was deleted successfully; <c>false</c> otherwise.
        /// </returns>
        bool UnloadSound(int soundID);
    }
}