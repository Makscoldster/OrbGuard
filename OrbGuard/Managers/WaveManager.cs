using OrbGuard.Core;
using OrbGuard.Managers;
using System;
using System.Collections.Generic;

namespace OrbGuard.Managers
{
    public class EnemySpawnEntry
    {
        public EnemyType Type { get; }
        public int Cost { get; }  // вартість в очках хвилі

        public EnemySpawnEntry(EnemyType type, int cost)
        {
            Type = type;
            Cost = cost;
        }
    }

    public class WaveManager
    {
        private readonly EnemyManager _enemyManager;
        private readonly List<EnemySpawnEntry> _catalog;

        private int _wavePoints;          // бюджет поточної хвилі
        private double _spawnInterval;    // секунд між спавнами
        private double _spawnTimer;       // таймер до наступного спавну
        private Queue<EnemyType> _spawnQueue; // черга ворогів для спавну

        public bool IsWaveActive { get; private set; }
        public bool IsWaveComplete => IsWaveActive &&
                                      _spawnQueue.Count == 0 &&
                                      !_enemyManager.HasEnemies();

        public WaveManager(EnemyManager enemyManager)
        {
            _enemyManager = enemyManager;
            _spawnQueue = new Queue<EnemyType>();
            _spawnInterval = 1.4;

            // каталог ворогів з їх вартістю
            _catalog = new List<EnemySpawnEntry>
            {
                new EnemySpawnEntry(EnemyType.Basic, 10),
                new EnemySpawnEntry(EnemyType.Fast,  5)
            };
        }

        public void StartWave(int waveNumber)
        {
            // бюджет росте з кожною хвилею
            _wavePoints = 50 + waveNumber * 50 + 2*(int)Math.Pow(waveNumber,3);
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

            // Fast з'являється тільки з хвилі 3
            float fastChance = waveNumber < 3 ? 0f : Math.Min((waveNumber - 2) * 0.15f, 0.5f);

            while (remaining > 0)
            {
                EnemySpawnEntry entry;
                if (remaining >= 18 && random.NextDouble() < fastChance)
                    entry = _catalog[1]; // Fast
                else
                    entry = _catalog[0]; // Basic

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

            if (_spawnTimer <= 0)
            {
                _enemyManager.Spawn(_spawnQueue.Dequeue());
                _spawnTimer = _spawnInterval;
            }

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