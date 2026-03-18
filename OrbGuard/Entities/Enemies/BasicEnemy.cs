using OrbGuard.Entities.Enemies;
using System.Windows;
using System.Windows.Media;

namespace OrbGuard.Entities.Enemies
{
    public class BasicEnemy : Enemy
    {
        private readonly Brush _brush = Brushes.Crimson;

        public BasicEnemy(List<Point> path)
            : base(
                x: path[0].X,
                y: path[0].Y,
                width: 24,
                height: 24,
                hp: 60,
                speed: 80,   // пікселів за секунду
                reward: 10,
                damage:10,

                path: path)
        { }

        public override void Render(DrawingContext dc)
        {
            dc.DrawEllipse(_brush, null, new Point(X, Y), Width / 2, Height / 2);
            RenderHpBar(dc);
        }
    }
}