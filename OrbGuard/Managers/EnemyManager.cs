using System.Windows;
using System.Windows.Media;
using OrbGuard.Entities.Enemies;
using OrbGuard.Map;

namespace OrbGuard.Managers
{
    public class EnemyManager
    {
        private readonly List<Enemy> _enemies = new();
        private readonly List<Point> _path;

        public IReadOnlyList<Enemy> Enemies => _enemies;

        public EnemyManager(GameMap map)
        {
            _path = map.Path;
        }

        // Фабричний метод — єдине місце де створюються вороги
        public Enemy Spawn(EnemyType type)
        {
            Enemy enemy = type switch
            {
                EnemyType.Basic => new BasicEnemy(_path),
                EnemyType.Fast => new FastEnemy(_path),
                _ => new BasicEnemy(_path)
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

        // Перевірка чи дійшов ворог до орба
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
        Fast
    }
}