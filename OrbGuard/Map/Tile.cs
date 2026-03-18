namespace OrbGuard.Map
{
    public enum TileType
    {
        Path,       // шлях ворогів — будувати не можна
        BuildZone,  // можна ставити башти
        OrbCenter,  // центр де знаходиться Orb
        Empty       // недоступна зона
    }

    public class Tile
    {
        public int Row { get; }
        public int Col { get; }
        public TileType Type { get; internal set; }
        public bool IsOccupied { get; private set; } // чи стоїть башта

        public double PixelX => Col * GameMap.TileSize;
        public double PixelY => Row * GameMap.TileSize;

        public Tile(int row, int col, TileType type)
        {
            Row = row;
            Col = col;
            Type = type;
            IsOccupied = false;
        }

        public bool CanBuild => Type == TileType.BuildZone && !IsOccupied;

        public void PlaceTower() => IsOccupied = true;
        public void RemoveTower() => IsOccupied = false;
    }
}