namespace OrbGuard.Map
{
    public enum TileType
    {
        Path,      
        BuildZone, 
        OrbCenter, 
        Empty      
    }

    public class Tile
    {
        public int Row { get; }
        public int Col { get; }
        public TileType Type { get; internal set; }
        public bool IsOccupied { get; private set; } 

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