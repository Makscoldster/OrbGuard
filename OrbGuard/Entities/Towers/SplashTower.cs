using OrbGuard.Entities.Enemies;
using System.Windows;
using System.Windows.Media;

namespace OrbGuard.Entities.Towers
{
    public class SplashTower : Tower
    {
        public double SplashRadius { get; private set; }
        private readonly Brush _brush = Brushes.OrangeRed;

        public SplashTower(double x, double y)
            : base(x, y,
                width: 36, height: 36,
                damage: 25, range: 72.5,
                fireRate: 1.2, cost: 200)
        {
            SplashRadius = 50;
        }

        protected override void Attack(Enemy target)
        {
            target.TakeDamage(Damage);
        }

        public void AttackSplash(Enemy target, List<Enemy> allEnemies, double deltaTime)
        {
            // використовуємо той самий cooldown що і в Tower
            if (!TickCooldown(deltaTime)) return; // повертає true тільки коли час стріляти

            foreach (var enemy in allEnemies)
            {
                if (!enemy.IsAlive) continue;
                double dx = enemy.X - target.X;
                double dy = enemy.Y - target.Y;
                double dist = Math.Sqrt(dx * dx + dy * dy);
                if (dist <= SplashRadius)
                    enemy.TakeDamage(Damage);
            }
        }

        public override void Render(DrawingContext dc)
        {
            // восьмикутник
            var geometry = new StreamGeometry();
            using (var ctx = geometry.Open())
            {
                double r = Width / 2;
                double s = r * 0.414; // зрізи кутів
                ctx.BeginFigure(new Point(X - s, Y - r), true, true);
                ctx.LineTo(new Point(X + s, Y - r), true, false);
                ctx.LineTo(new Point(X + r, Y - s), true, false);
                ctx.LineTo(new Point(X + r, Y + s), true, false);
                ctx.LineTo(new Point(X + s, Y + r), true, false);
                ctx.LineTo(new Point(X - s, Y + r), true, false);
                ctx.LineTo(new Point(X - r, Y + s), true, false);
                ctx.LineTo(new Point(X - r, Y - s), true, false);
            }
            geometry.Freeze();
            dc.DrawGeometry(_brush, null, geometry);
            RenderRange(dc);
        }
    }
}