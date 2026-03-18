using System.Windows;
using System.Windows.Media;

namespace OrbGuard.Entities.Enemies
{
    namespace OrbGuard.Entities.Enemies
    {
        public class BasicEnemy : Enemy
        {
            private readonly Brush _brush = Brushes.Crimson;

            public static int StaticCost => 10;
            public static Func<int, float> GetSpawnRule() => wave => 1.0f;
            public static Func<int, int> GetRewardRule() => wave => Math.Max(3, 8 - wave);

            public override Func<int, float> SpawnRule => GetSpawnRule();
            public override Func<int, int> RewardRule => GetRewardRule();

            public BasicEnemy(List<Point> path, int waveNumber = 1)
                : base(x: path[0].X, y: path[0].Y,
                       width: 24, height: 24,
                       hp: 100, speed: 100,
                       reward: GetRewardRule()(waveNumber),
                       damage: 5,
                       cost: StaticCost,
                       path: path)
            { }

            public override void Render(DrawingContext dc)
            {
                dc.DrawEllipse(_brush, null, new Point(X, Y), Width / 2, Height / 2);
                RenderHpBar(dc);
            }
        }
    }
}