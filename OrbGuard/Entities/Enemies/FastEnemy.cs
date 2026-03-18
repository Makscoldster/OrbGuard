using OrbGuard.Entities.Enemies;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace OrbGuard.Entities.Enemies
{
    public class FastEnemy : Enemy
    {
        private readonly Brush _brush = Brushes.Yellow;

        public FastEnemy(List<Point> path)
            : base(
                x: path[0].X,
                y: path[0].Y,
                width: 16,
                height: 16,
                hp: 50,
                speed: 200,  
                reward: 10,  
                damage: 5,
                path: path)
        { }

        public override void Render(DrawingContext dc)
        {
            // ромб замість кола — візуально відрізняється
            var geometry = new StreamGeometry();
            using (var ctx = geometry.Open())
            {
                ctx.BeginFigure(new Point(X, Y - Height / 2), true, true);
                ctx.LineTo(new Point(X + Width / 2, Y), true, false);
                ctx.LineTo(new Point(X, Y + Height / 2), true, false);
                ctx.LineTo(new Point(X - Width / 2, Y), true, false);
            }
            geometry.Freeze(); // оптимізація — об'єкт незмінний, WPF кешує
            dc.DrawGeometry(_brush, null, geometry);
            RenderHpBar(dc);
        }
    }
}
