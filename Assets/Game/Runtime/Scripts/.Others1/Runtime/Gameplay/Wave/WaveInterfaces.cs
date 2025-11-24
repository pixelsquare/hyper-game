using UnityEngine;

namespace Santelmo.Rinsurv
{
    public interface IWaveInterval
    {
        public float WaveInterval { get; }
        public SpawnData[] GetIntervalWave(Vector2 origin);
    }

    public interface IWaveInitial
    {
        public SpawnData[] GetInitialWave(Vector2 origin);
    }

    public interface IWaveFinale
    {
        public void OnWaveFinale();
    }
}
