using OrbGuard.Core;
using OrbGuard.Entities;
using System.Windows;
using System.Windows.Media;

namespace OrbGuard.Entities.Enemies
{
    public abstract class Enemy : GameObject
    {
        public double MaxHp { get; private set; }
        public double CurrentHp { get; private set; }
        public double Speed { get; protected set; }
        public int Reward { get; private set; }      // золото за вбивство
        public int PathIndex { get; private set; }   // поточна точка шляху
        public double Damage { get; protected set; } // домаг коли ворог доходить до орба

        private readonly List<Point> _path;

        public bool ReachedOrb => PathIndex >= _path.Count;

        protected Enemy(double x, double y, double width, double height,
                        double hp, double speed, int reward, double damage, List<Point> path)
            : base(x, y, width, height)
        {
            MaxHp = hp;
            CurrentHp = hp;
            Speed = speed;
            Reward = reward;
            Damage = damage;
            PathIndex = 0;
            _path = path;

        }

        public void TakeDamage(double amount)
        {
            CurrentHp = Math.Max(0, CurrentHp - amount);
            if (CurrentHp <= 0)
                Die();
        }

        private void Die()
        {
            GameManager.Instance.AddGold(Reward);
            Destroy();
        }

        public override void Update(double deltaTime)
        {
            if (!IsAlive || ReachedOrb) return;
            MoveAlongPath(deltaTime);
        }

        private void MoveAlongPath(double deltaTime)
        {
            Point target = _path[PathIndex];
            double dx = target.X - X;
            double dy = target.Y - Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);

            double moveDistance = Speed * deltaTime;

            if (distance <= moveDistance)
            {
                // досягли точки — переходимо до наступної
                X = target.X;
                Y = target.Y;
                PathIndex++;
            }
            else
            {
                // рухаємось у напрямку точки
                X += dx / distance * moveDistance;
                Y += dy / distance * moveDistance;
            }
        }

        // Рендер HP бару — спільний для всіх ворогів
        protected void RenderHpBar(DrawingContext dc)
        {
            double barWidth = Width;
            double barHeight = 4;
            double barY = Y - Height / 2 - 6;
            double barX = X - Width / 2;

            // фон бару
            dc.DrawRectangle(Brushes.DarkRed, null,
                new Rect(barX, barY, barWidth, barHeight));

            // поточне HP
            double hpRatio = CurrentHp / MaxHp;
            dc.DrawRectangle(Brushes.LimeGreen, null,
                new Rect(barX, barY, barWidth * hpRatio, barHeight));
        }

        // Нащадки самі вирішують як виглядати
        public override abstract void Render(DrawingContext dc);
    }
}
