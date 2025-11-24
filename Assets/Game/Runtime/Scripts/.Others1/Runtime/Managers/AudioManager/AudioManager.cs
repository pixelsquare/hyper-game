using System;
using DarkTonic.MasterAudio;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class AudioManager : IAudioManager
    {
        public float BgmVolume => _bgmVolume;
        public float SfxVolume => _sfxVolume;

        private float _bgmVolume = 1.0f;
        private float _sfxVolume = 1.0f;

        private const string SfxBusName = "SFX";
        private const string BgmPlaylist = "BGMPlaylist";

        public void PlaySound(Sfx sfx)
        {
            PlaySound(sfx.ToStringValue(), true);
        }

        public void PlaySound(Bgm bgm)
        {
            PlaySound(bgm.ToStringValue(), false);
        }

        public void PlaySound(string clipName, bool isSfx)
        {
            if (!isSfx)
            {
                MasterAudio.StartPlaylistOnClip(BgmPlaylist, clipName);
                return;
            }

            if (!MasterAudio.PlaySoundAndForget(clipName, volumePercentage: _sfxVolume))
            {
                throw new Exception($"SFX failed to play. {clipName}");
            }
        }

        public void PauseSound(Bgm bgm)
        {
            MasterAudio.PausePlaylist(BgmPlaylist);
        }

        public void StopSound(Bgm bgm)
        {
            MasterAudio.StopPlaylist(BgmPlaylist);
        }
        
        public void StopSound(Sfx sfx)
        {
            MasterAudio.StopAllOfSound(sfx.ToStringValue());
        }

        public void SetSfxVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);
            PersistentAudioSettings.SetBusVolume("SFX", _sfxVolume);
        }

        public void SetBgmVolume(float volume)
        {
            _bgmVolume = Mathf.Clamp01(volume);
            PersistentAudioSettings.MusicVolume = _bgmVolume;
        }
    }
}
