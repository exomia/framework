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
using SharpDX;
using SharpDX.X3DAudio;

namespace Exomia.Framework.Audio
{
    /// <inheritdoc cref="IDisposable" />
    /// <summary>
    ///     IAudioManager interface
    /// </summary>
    public interface IAudioManager : IDisposable
    {
        /// <summary>
        ///     BGMVolume
        /// </summary>
        float BgmVolume { get; set; }

        /// <summary>
        ///     EnvironmentVolume
        /// </summary>
        float EnvironmentVolume { get; set; }

        /// <summary>
        ///     FxVolume
        /// </summary>
        float FxVolume { get; set; }

        /// <summary>
        ///     MasterVolume
        /// </summary>
        float MasterVolume { get; set; }

        /// <summary>
        ///     ListenerPosition
        /// </summary>
        Vector3 ListenerPosition { get; set; }

        /// <summary>
        ///     ListenerVelocity
        /// </summary>
        Vector3 ListenerVelocity { get; set; }

        /// <summary>
        ///     load a sound from a given stream and returns the sound id
        /// </summary>
        /// <param name="stream">sound stream to load the sound from</param>
        /// <returns>sound id</returns>
        int LoadSound(Stream stream);

        /// <summary>
        ///     pause a bgm song
        /// </summary>
        void PauseBgm();

        /// <summary>
        ///     pause all environment sounds
        /// </summary>
        void PauseEnvSounds();

        /// <summary>
        ///     pause all fx sounds
        /// </summary>
        void PauseFxSounds();

        /// <summary>
        ///     play an environment sound
        /// </summary>
        /// <param name="soundID">the sound id</param>
        /// <param name="emitterPos">emitter position</param>
        /// <param name="volume">volume</param>
        /// <param name="maxDistance">max distance to hear the sound</param>
        /// <param name="onFxEnd">called than the sound ends</param>
        void PlayEnvSound(int soundID, Vector3 emitterPos, float volume, float maxDistance,
            Action<IntPtr> onFxEnd = null);

        /// <summary>
        ///     play a fx sound
        /// </summary>
        /// <param name="soundID">the sound id</param>
        /// <param name="emitterPos">emitter position</param>
        /// <param name="volume">volume</param>
        /// <param name="maxDistance">max distance to hear the sound</param>
        /// <param name="onFxEnd">called than the sound ends</param>
        void PlayFxSound(int soundID, Vector3 emitterPos, float volume, float maxDistance,
            Action<IntPtr> onFxEnd = null);

        /// <summary>
        ///     play an environment sound
        /// </summary>
        /// <param name="soundID">the sound id</param>
        /// <param name="emitter">the emitter</param>
        /// <param name="volume">volume</param>
        /// <param name="onFxEnd">called than the sound ends</param>
        void PlayEnvSound(int soundID, Emitter emitter, float volume, Action<IntPtr> onFxEnd = null);

        /// <summary>
        ///     play a fx sound
        /// </summary>
        /// <param name="soundID">the sound id</param>
        /// <param name="emitter">the emitter</param>
        /// <param name="volume">volume</param>
        /// <param name="onFxEnd">called than the sound ends</param>
        void PlayFxSound(int soundID, Emitter emitter, float volume, Action<IntPtr> onFxEnd = null);

        /// <summary>
        ///     resume a bgm song
        /// </summary>
        void ResumeBgm();

        /// <summary>
        ///     resume all environment sounds
        /// </summary>
        void ResumeEnvSounds();

        /// <summary>
        ///     resume all fx sounds
        /// </summary>
        void ResumeFxSounds();

        /// <summary>
        ///     starts a back ground music song
        /// </summary>
        /// <param name="songID">the song id</param>
        /// <param name="onBgmEnd">called than the song ends</param>
        void RunBgm(int songID, Action<IntPtr> onBgmEnd = null);

        /// <summary>
        ///     stop a bgm song
        /// </summary>
        void StopBgm();

        /// <summary>
        ///     stops all environment sounds
        /// </summary>
        void StopEnvSounds();

        /// <summary>
        ///     stops all fx sounds
        /// </summary>
        void StopFxSounds();

        /// <summary>
        ///     unload all loaded sounds and free resources
        /// </summary>
        void UnloadAll();

        /// <summary>
        ///     unload a sound and free resources from a given sound id
        /// </summary>
        /// <param name="soundID">the sound id</param>
        /// <returns><c>true</c>if sound was deleted successfully; <c>false</c> otherwise.</returns>
        bool UnloadSound(int soundID);
    }
}