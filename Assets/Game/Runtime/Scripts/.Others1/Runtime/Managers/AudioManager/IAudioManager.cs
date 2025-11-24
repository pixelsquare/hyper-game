namespace Santelmo.Rinsurv
{
    public interface IAudioManager : IGlobalBinding
    {
        public float BgmVolume { get; }
        public float SfxVolume { get; }

        public void PlaySound(Sfx sfx);
        public void PlaySound(Bgm bgm);
        public void PlaySound(string clipName, bool isSfx);

        public void PauseSound(Bgm bgm);
        public void StopSound(Bgm bgm);
        public void StopSound(Sfx sfx);
        
        public void SetSfxVolume(float volume);
        public void SetBgmVolume(float volume);
    }
}
