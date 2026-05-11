using OrbGuard.Entities.Enemies;
using OrbGuard.Entities.Enemies.OrbGuard.Entities.Enemies;
using OrbGuard.Map;
using System.Windows;
using System.Windows.Media;

namespace OrbGuard.Managers
{
    public class EnemyManager
    {
        private readonly List<Enemy> _enemies = new();
        private readonly List<Point> _path;

        public List<Enemy> Enemies => _enemies;

        public EnemyManager(GameMap map)
        {
            _path = map.Path;
        }

        public Enemy Spawn(EnemyType type, int waveNumber = 1)
        {
            Enemy enemy = type switch
            {
                EnemyType.Basic => new BasicEnemy(_path, waveNumber),
                EnemyType.Fast => new FastEnemy(_path, waveNumber),
                EnemyType.Tank => new TankEnemy(_path, waveNumber),
                _ => new BasicEnemy(_path, waveNumber)
            };
            _enemies.Add(enemy);
            return enemy;
        }

        public void UpdateAll(double deltaTime)
        {
            foreach (var enemy in _enemies)
                enemy.Update(deltaTime);

            RemoveDead();
        }

        public void RenderAll(DrawingContext dc)
        {
            foreach (var enemy in _enemies)
                enemy.Render(dc);
        }

        public List<Enemy> GetEnemiesAtOrb()
        {
            var reached = new List<Enemy>();
            foreach (var enemy in _enemies)
                if (enemy.ReachedOrb && enemy.IsAlive)
                    reached.Add(enemy);
            return reached;
        }

        public bool HasEnemies() => _enemies.Count > 0;

        private void RemoveDead()
        {
            _enemies.RemoveAll(e => !e.IsAlive);
        }

        public void Clear() => _enemies.Clear();
    }

    public enum EnemyType
    {
        Basic,
        Fast,
        Tank
    }
}