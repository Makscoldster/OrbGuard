using System.Windows;
using System.Windows.Media;

namespace OrbGuard.Map
{
    public class GameMap
    {
        public const int TileSize = 48;     // розмір тайлу в пікселях
        public const int Rows = 13;
        public const int Cols = 21;

        public Tile[,] Tiles { get; private set; }
        public List<Point> Path { get; private set; } // шлях у пікселях

        // Кольори тайлів
        private static readonly Brush PathBrush = new SolidColorBrush(Color.FromRgb(180, 140, 80));
        private static readonly Brush BuildBrush = new SolidColorBrush(Color.FromRgb(60, 100, 60));
        private static readonly Brush OrbBrush = new SolidColorBrush(Color.FromRgb(30, 30, 80));
        private static readonly Brush EmptyBrush = new SolidColorBrush(Color.FromRgb(40, 40, 40));

        public GameMap()
        {
            Tiles = new Tile[Rows, Cols];
            Path = new List<Point>();
            InitializeMap();
            BuildPath();
        }

        private void InitializeMap()
        {
            // заповнюємо все як BuildZone
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    Tiles[r, c] = new Tile(r, c, TileType.BuildZone);

            // шлях ворогів — змійка через карту до центру
            int[] pathRows = { 2, 2, 6, 6, 10, 10, 6 };
            int[] pathColStart = { 0, 1, 19, 1, 1, 19, 9 };
            int[] pathColEnd = { 1, 19, 19, 1, 19, 10, 10 };

            // горизонтальні відрізки
            MarkPathRow(2, 0, 20);   // зліва направо
            MarkPathRow(6, 0, 20);   // справа наліво (змійка)
            MarkPathRow(10, 0, 20);   // зліва направо

            // вертикальні з'єднання
            MarkPathCol(20, 2, 6);     // з'єднуємо рядки 2→6
            MarkPathCol(0, 6, 10);    // з'єднуємо рядки 6→10
            MarkPathCol(10, 6, 10);    // фінальний поворот до орба

            // центр — орб
            Tiles[6, 10].Type = TileType.OrbCenter;
        }

        private void MarkPathRow(int row, int colFrom, int colTo)
        {
            int step = colFrom < colTo ? 1 : -1;
            for (int c = colFrom; c != colTo + step; c += step)
                Tiles[row, c].Type = TileType.Path;
        }

        private void MarkPathCol(int col, int rowFrom, int rowTo)
        {
            for (int r = rowFrom; r <= rowTo; r++)
                Tiles[r, col].Type = TileType.Path;
        }

        private void BuildPath()
        {
            // список точок шляху у пікселях — вороги йдуть по цих точках
            Path.Add(TileCenter(2, 0));    // старт
            Path.Add(TileCenter(2, 20));   // →
            Path.Add(TileCenter(6, 20));   // ↓
            Path.Add(TileCenter(6, 0));    // ←
            Path.Add(TileCenter(10, 0));   // ↓
            Path.Add(TileCenter(10, 20));  // →
            Path.Add(TileCenter(6, 10));   // фінал — орб
        }

        private Point TileCenter(int row, int col)
        {
            return new Point(
                col * TileSize + TileSize / 2.0,
                row * TileSize + TileSize / 2.0);
        }

        public Tile? GetTileAtPixel(double px, double py)
        {
            int col = (int)(px / TileSize);
            int row = (int)(py / TileSize);

            if (row < 0 || row >= Rows || col < 0 || col >= Cols)
                return null;

            return Tiles[row, col];
        }

        public void Render(DrawingContext dc)
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    Brush brush = Tiles[r, c].Type switch
                    {
                        TileType.Path => PathBrush,
                        TileType.BuildZone => BuildBrush,
                        TileType.OrbCenter => OrbBrush,
                        _ => EmptyBrush
                    };

                    dc.DrawRectangle(brush, null,
                        new Rect(c * TileSize, r * TileSize, TileSize, TileSize));
                }
            }
        }
    }
}