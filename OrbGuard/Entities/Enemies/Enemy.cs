using OrbGuard.Core;
using OrbGuard.Entities;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace OrbGuard.Entities.Enemies
{
    public abstract class Enemy : GameObject
    {
        public double MaxHp { get; private set; }
        public double CurrentHp { get; private set; }
        public double Speed { get; protected set; }
        public int Reward { get; private set; }
        public int PathIndex { get; private set; }
        public double Damage { get; protected set; }
        public int Cost { get; protected set; }

        private readonly List<Point> _path;
        public bool ReachedOrb => PathIndex >= _path.Count;
        public abstract Func<int, float> SpawnRule { get; }
        public abstract Func<int, int> RewardRule { get; }

        protected Enemy(double x, double y, double width, double height,
                        double hp, double speed, int reward, double damage,
                        int cost, List<Point> path)
            : base(x, y, width, height)
        {
            MaxHp = hp;
            CurrentHp = hp;
            Speed = speed;
            Reward = reward;
            Damage = damage;
            Cost = cost;
            PathIndex = 0;
            _path = path;
        }

        public void TakeDamage(double amount)
        {
            CurrentHp = Math.Max(0, CurrentHp - amount);
            if (CurrentHp <= 0) Die();
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
                X = target.X;
                Y = target.Y;
                PathIndex++;
            }
            else
            {
                X += dx / distance * moveDistance;
                Y += dy / distance * moveDistance;
            }
        }

        protected void RenderHpBar(DrawingContext dc)
        {
            if (CurrentHp >= MaxHp) return;
            double barWidth = Width;
            double barHeight = 4;
            double barY = Y - Height / 2 - 6;
            double barX = X - Width / 2;

            dc.DrawRectangle(Brushes.DarkRed, null,
                new Rect(barX, barY, barWidth, barHeight));

            double hpRatio = CurrentHp / MaxHp;
            dc.DrawRectangle(Brushes.LimeGreen, null,
                new Rect(barX, barY, barWidth * hpRatio, barHeight));
        }

        public override abstract void Render(DrawingContext dc);
    }
}