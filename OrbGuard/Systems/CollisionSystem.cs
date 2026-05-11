using OrbGuard.Core;
using OrbGuard.Entities;
using OrbGuard.Entities.Enemies;

namespace OrbGuard.Systems
{
    public class CollisionSystem
    {
        private readonly GameManager _gameManager;

        public CollisionSystem()
        {
            _gameManager = GameManager.Instance;
        }

        public void CheckEnemiesAtOrb(List<Enemy> enemies, Orb orb)
        {
            foreach (var enemy in enemies)
            {
                if (!enemy.IsAlive) continue;
                if (!enemy.ReachedOrb) continue;

                orb.TakeDamage(enemy.Damage);
                enemy.Destroy();

                if (orb.IsDestroyed)
                {
                    _gameManager.TriggerGameOver();
                    return;
                }
            }
        }

        public void CheckVictory(int totalWaves)
        {
            if (GameManager.Instance.CurrentWave >= totalWaves &&
                GameManager.Instance.CurrentPhase == GamePhase.Preparing)
            {
                _gameManager.TriggerVictory();
            }
        }
    }
}