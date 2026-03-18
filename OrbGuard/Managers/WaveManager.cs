using OrbGuard.Core;
using OrbGuard.Entities.Enemies;
using OrbGuard.Entities.Enemies.OrbGuard.Entities.Enemies;
using OrbGuard.Managers;
using System;
using System.Collections.Generic;

namespace OrbGuard.Managers
{
    public class EnemySpawnEntry
    {
        public EnemyType Type { get; }
        public int Cost { get; }
        public Func<int, float> SpawnRule { get; }
        public Func<int, int> RewardRule { get; }

        public EnemySpawnEntry(EnemyType type, int cost,
                               Func<int, float> spawnRule,
                               Func<int, int> rewardRule)
        {
            Type = type;
            Cost = cost;
            SpawnRule = spawnRule;
            RewardRule = rewardRule;
        }

        public float GetChance(int wave) => SpawnRule(wave);
        public int GetReward(int wave) => RewardRule(wave);
    }

    public class WaveManager
    {
        private readonly EnemyManager _enemyManager;
        private readonly List<EnemySpawnEntry> _catalog;

        private double _spawnInterval;
        private double _spawnTimer;
        private int _wavePoints;
        private int _currentWaveNumber;
        private Queue<EnemyType> _spawnQueue;

        public bool IsWaveActive { get; private set; }
        public bool IsWaveComplete => IsWaveActive &&
                                      _spawnQueue.Count == 0 &&
                                      !_enemyManager.HasEnemies();

        public WaveManager(EnemyManager enemyManager)
        {
            _enemyManager = enemyManager;
            _spawnQueue = new Queue<EnemyType>();

            _catalog = new List<EnemySpawnEntry>
            {
                new EnemySpawnEntry(EnemyType.Basic, BasicEnemy.StaticCost,
                                    BasicEnemy.GetSpawnRule(), BasicEnemy.GetRewardRule()),
                new EnemySpawnEntry(EnemyType.Fast,  FastEnemy.StaticCost,
                                    FastEnemy.GetSpawnRule(),  FastEnemy.GetRewardRule()),
                new EnemySpawnEntry(EnemyType.Tank,  TankEnemy.StaticCost,
                                    TankEnemy.GetSpawnRule(),  TankEnemy.GetRewardRule())
            };
        }

        public void StartWave(int waveNumber)
        {
            _currentWaveNumber = waveNumber;
            _wavePoints = 50 + waveNumber * 50 + 2 * (int)Math.Pow(waveNumber, 3);
            _spawnQueue = BuildSpawnQueue(_wavePoints, waveNumber);
            _spawnInterval = Math.Max(0.2, 1.4 - waveNumber * 0.3);
            _spawnTimer = 0;
            IsWaveActive = true;

            GameManager.Instance.StartWave();
        }

        private Queue<EnemyType> BuildSpawnQueue(int points, int waveNumber)
        {
            var queue = new Queue<EnemyType>();
            var random = new Random();
            int remaining = points;

            while (remaining > 0)
            {
                float tankChance = _catalog[2].GetChance(waveNumber);
                float fastChance = _catalog[1].GetChance(waveNumber);
                double roll = random.NextDouble();

                EnemySpawnEntry entry;
                if (roll < tankChance && remaining >= TankEnemy.StaticCost)
                    entry = _catalog[2];
                else if (roll < tankChance + fastChance && remaining >= FastEnemy.StaticCost)
                    entry = _catalog[1];
                else
                    entry = _catalog[0];

                if (entry.Cost > remaining) break;
                queue.Enqueue(entry.Type);
                remaining -= entry.Cost;
            }

            return queue;
        }

        public void Update(double deltaTime)
        {
            if (!IsWaveActive || _spawnQueue.Count == 0) return;

            _spawnTimer -= deltaTime;
            if (_spawnTimer > 0) return;

            _enemyManager.Spawn(_spawnQueue.Dequeue(), _currentWaveNumber);
            _spawnTimer = _spawnInterval;

            if (IsWaveComplete)
                OnWaveComplete();
        }

        private void OnWaveComplete()
        {
            IsWaveActive = false;
            GameManager.Instance.OnWaveComplete();
        }
    }
}