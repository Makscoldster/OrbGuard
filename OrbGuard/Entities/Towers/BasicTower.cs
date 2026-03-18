using OrbGuard.Entities.Enemies;
using System.Windows;
using System.Windows.Media;

namespace OrbGuard.Entities.Towers
{
    public class BasicTower : Tower
    {
        private readonly Brush _brush = Brushes.SteelBlue;

        public BasicTower(double x, double y)
            : base(x, y,
                width: 32, height: 32,
                damage: 20, range: 120,
                fireRate: 1.5, cost: 100)
        { }

        protected override void Attack(Enemy target)
        {
            // миттєвий урон — снаряди додамо в ProjectileManager пізніше
            target.TakeDamage(Damage);
        }

        public override void Render(DrawingContext dc)
        {
            dc.DrawRectangle(_brush, null,
                new Rect(X - Width / 2, Y - Height / 2, Width, Height));
            RenderRange(dc);
        }
    }
}