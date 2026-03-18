using OrbGuard.Entities.Enemies;
using System.Windows;
using System.Windows.Media;

namespace OrbGuard.Entities.Towers
{
    public class SniperTower : Tower
    {
        private readonly Brush _brush = Brushes.DarkGreen;

        public SniperTower(double x, double y)
            : base(x, y,
                width: 28, height: 28,
                damage: 80, range: 250,
                fireRate: 0.5, cost: 150)
        { }

        protected override void Attack(Enemy target)
        {
            target.TakeDamage(Damage);
        }

        public override void Render(DrawingContext dc)
        {
            // трикутник — візуально відрізняється від BasicTower
            var geometry = new StreamGeometry();
            using (var ctx = geometry.Open())
            {
                ctx.BeginFigure(new Point(X, Y - Height / 2), true, true);
                ctx.LineTo(new Point(X + Width / 2, Y + Height / 2), true, false);
                ctx.LineTo(new Point(X - Width / 2, Y + Height / 2), true, false);
            }
            geometry.Freeze();
            dc.DrawGeometry(_brush, null, geometry);
            RenderRange(dc);
        }
    }
}