using OrbGuard.Entities.Enemies;
using System.Windows;
using System.Windows.Media;

namespace OrbGuard.Entities.Towers
{
    public abstract class Tower : GameObject
    {
        public double Damage { get; protected set; }
        public double Range { get; protected set; }
        public double FireRate { get; protected set; } 
        public int Cost { get; private set; }

        private double _fireCooldown;
        public Enemy? CurrentTarget { get; private set; }

        protected Tower(double x, double y, double width, double height,
                        double damage, double range, double fireRate, int cost)
            : base(x, y, width, height)
        {
            Damage = damage;
            Range = range;
            FireRate = fireRate;
            Cost = cost;
            _fireCooldown = 0;
        }


        public void AcquireTarget(List<Enemy> enemies)
        {
            CurrentTarget = null;
            double closestDist = double.MaxValue;

            foreach (var enemy in enemies)
            {
                if (!enemy.IsAlive) continue;

                double dist = GetDistanceTo(enemy);
                if (dist <= Range && dist < closestDist)
                {
                    closestDist = dist;
                    CurrentTarget = enemy;
                }
            }
        }

        private bool IsTargetValid(Enemy target)
        {
            return target.IsAlive && GetDistanceTo(target) <= Range;
        }

        private double GetDistanceTo(Enemy enemy)
        {
            double dx = enemy.X - X;
            double dy = enemy.Y - Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        protected void RenderRange(DrawingContext dc)
        {
            var pen = new Pen(Brushes.White, 0.5) { DashStyle = DashStyles.Dash };
            dc.PushClip(new RectangleGeometry(new Rect(0, 0, 1008, 624)));
            dc.DrawEllipse(null, pen, new Point(X, Y), Range, Range);
            dc.Pop();
        }
        protected bool TickCooldown(double deltaTime)
        {
            _fireCooldown -= deltaTime;
            if (_fireCooldown <= 0)
            {
                _fireCooldown = 1.0 / FireRate;
                return true;
            }
            return false;
        }

        public override void Update(double deltaTime)
        {
            if (!IsAlive) return;
            if (CurrentTarget == null || !IsTargetValid(CurrentTarget))
                CurrentTarget = null;

            if (CurrentTarget != null && TickCooldown(deltaTime))
                Attack(CurrentTarget);
        }

        protected abstract void Attack(Enemy target);

        public override abstract void Render(DrawingContext dc);
    }
}