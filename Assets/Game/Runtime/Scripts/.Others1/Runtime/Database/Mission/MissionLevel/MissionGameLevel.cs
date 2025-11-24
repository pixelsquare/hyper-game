using System;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [Serializable]
    public struct MissionGameLevel
    {
        public GameObject _environment;
        public EnemyWaveConfig[] _enemyWaves;
        public MissionStatsModifier _enemyStatsModifier;
        public float _maxTime;
    }

    [Serializable]
    public struct MissionStatsModifier
    {
        public float _health;
        public float _damage;
        public float _movespeed;
        public float _lootChance;
    }
}
