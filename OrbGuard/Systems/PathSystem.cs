using OrbGuard.Core;
using OrbGuard.Systems;
using System.Collections.Generic;
using System.Windows;

namespace OrbGuard.Systems
{
    public class PathSystem
    {
        private readonly List<Point> _waypoints;

        public IReadOnlyList<Point> Waypoints => _waypoints;
        public int WaypointCount => _waypoints.Count;

        public PathSystem(List<Point> initialPath)
        {
            _waypoints = new List<Point>(initialPath);
        }

        public Point? GetWaypoint(int index)
        {
            if (index < 0 || index >= _waypoints.Count)
                return null;
            return _waypoints[index];
        }

        public bool IsEndOfPath(int index) => index >= _waypoints.Count;

        public double GetTotalLength()
        {
            double total = 0;
            for (int i = 1; i < _waypoints.Count; i++)
            {
                double dx = _waypoints[i].X - _waypoints[i - 1].X;
                double dy = _waypoints[i].Y - _waypoints[i - 1].Y;
                total += System.Math.Sqrt(dx * dx + dy * dy);
            }
            return total;
        }
    }
}