using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace OrbGuard.Entities
{
    public abstract class GameObject
    {
        public double X { get; protected set; }
        public double Y { get; protected set; }
        public double Width { get; protected set; }
        public double Height { get; protected set; }
        public bool IsAlive { get; protected set; } = true;

        protected GameObject(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public abstract void Update(double deltaTime);
        public abstract void Render(DrawingContext dc);

        public virtual void Destroy()
        {
            IsAlive = false;
        }
    }
}
