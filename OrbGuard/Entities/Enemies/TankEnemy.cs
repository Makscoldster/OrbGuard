using OrbGuard.Entities.Enemies;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace OrbGuard.Entities.Enemies
{
    public class TankEnemy : Enemy
    {
        private readonly Brush _brush = Brushes.DarkViolet;

        public static int StaticCost => 50;
        public static Func<int, float> GetSpawnRule() =>
            wave => wave < 5 ? 0f : Math.Min((wave - 4) * 0.08f, 0.3f);
        public static Func<int, int> GetRewardRule() => wave => Math.Max(4, 21 - wave*3);

        public override Func<int, float> SpawnRule => GetSpawnRule();
        public override Func<int, int> RewardRule => GetRewardRule();

        public TankEnemy(List<Point> path, int waveNumber = 1)
            : base(x: path[0].X, y: path[0].Y,
                   width: 36, height: 36,
                   hp: 400, speed: 75,
                   reward: GetRewardRule()(waveNumber),
                   damage: 10,
                   cost: StaticCost,
                   path: path)
        { }

        public override void Render(DrawingContext dc)
        {
            var geometry = new StreamGeometry();
            using (var ctx = geometry.Open())
            {
                double r = Width / 2;
                ctx.BeginFigure(new Point(X, Y - r), true, true);
                ctx.LineTo(new Point(X + r * 0.866, Y - r * 0.5), true, false);
                ctx.LineTo(new Point(X + r * 0.866, Y + r * 0.5), true, false);
                ctx.LineTo(new Point(X, Y + r), true, false);
                ctx.LineTo(new Point(X - r * 0.866, Y + r * 0.5), true, false);
                ctx.LineTo(new Point(X - r * 0.866, Y - r * 0.5), true, false);
            }
            geometry.Freeze();
            dc.DrawGeometry(_brush, null, geometry);
            RenderHpBar(dc);
        }
    }
}
