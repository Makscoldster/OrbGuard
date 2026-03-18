using System.Windows;
using System.Windows.Media;

namespace OrbGuard.Entities.Enemies
{
    public class FastEnemy : Enemy
    {
        private readonly Brush _brush = Brushes.Yellow;

        public static int StaticCost => 18;
        public static Func<int, float> GetSpawnRule() =>
            wave => wave < 3 ? 0f : Math.Min((wave - 2) * 0.15f, 0.4f);
        public static Func<int, int> GetRewardRule() => wave => Math.Max(2, 9 - wave);

        public override Func<int, float> SpawnRule => GetSpawnRule();
        public override Func<int, int> RewardRule => GetRewardRule();

        public FastEnemy(List<Point> path, int waveNumber = 1)
            : base(x: path[0].X, y: path[0].Y,
                   width: 16, height: 16,
                   hp: 50, speed: 200,
                   reward: GetRewardRule()(waveNumber),
                   damage: 5,
                   cost: StaticCost,
                   path: path)
        { }

        public override void Render(DrawingContext dc)
        {
            var geometry = new StreamGeometry();
            using (var ctx = geometry.Open())
            {
                ctx.BeginFigure(new Point(X, Y - Height / 2), true, true);
                ctx.LineTo(new Point(X + Width / 2, Y), true, false);
                ctx.LineTo(new Point(X, Y + Height / 2), true, false);
                ctx.LineTo(new Point(X - Width / 2, Y), true, false);
            }
            geometry.Freeze();
            dc.DrawGeometry(_brush, null, geometry);
            RenderHpBar(dc);
        }
    }
}
