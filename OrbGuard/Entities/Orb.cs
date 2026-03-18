using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace OrbGuard.Entities
{
    public class Orb : GameObject
    {
        public double MaxHp { get; private set; }
        public double CurrentHp { get; private set; }
        public double RegenRate { get; private set; } // HP за секунду

        private readonly Brush _normalBrush = Brushes.DeepSkyBlue;
        private readonly Brush _damagedBrush = Brushes.OrangeRed;

        public bool IsDestroyed => CurrentHp <= 0;

        public Orb(double x, double y) : base(x, y, 40, 40)
        {
            MaxHp = 100;
            CurrentHp = 100;
            RegenRate = 0.1;
        }

        public void TakeDamage(double amount)
        {
            CurrentHp = Math.Max(0, CurrentHp - amount);
            if (CurrentHp <= 0)
                Destroy();
        }

        public void Regenerate(double deltaTime)
        {
            if (!IsAlive) return;
            CurrentHp = Math.Min(MaxHp, CurrentHp + RegenRate * deltaTime);
        }

        public override void Update(double deltaTime)
        {
            Regenerate(deltaTime);
        }

        public override void Render(DrawingContext dc)
        {
            Brush brush = CurrentHp < MaxHp * 0.3 ? _damagedBrush : _normalBrush;
            dc.DrawEllipse(brush, null, new Point(X, Y), Width / 2, Height / 2);
        }
    }
}
