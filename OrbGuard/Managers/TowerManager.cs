using System;
using System.Windows.Media;
using OrbGuard.Core;
using OrbGuard.Entities.Enemies;
using OrbGuard.Entities.Towers;
using OrbGuard.Map;

namespace OrbGuard.Managers
{
    public class TowerManager
    {
        private readonly List<Tower> _towers = new();
        private readonly GameMap _map;

        public IReadOnlyList<Tower> Towers => _towers;

        public TowerManager(GameMap map)
        {
            _map = map;
        }

        public bool TryPlaceTower(TowerType type, double pixelX, double pixelY)
        {
            Tile? tile = _map.GetTileAtPixel(pixelX, pixelY);

            if (tile == null || !tile.CanBuild)
                return false;

            int cost = GetCost(type);
            if (!GameManager.Instance.SpendGold(cost))
                return false;

            Tower tower = type switch
            {
                TowerType.Basic => new BasicTower(tile.PixelX + GameMap.TileSize / 2.0,
                                                   tile.PixelY + GameMap.TileSize / 2.0),
                TowerType.Sniper => new SniperTower(tile.PixelX + GameMap.TileSize / 2.0,
                                                    tile.PixelY + GameMap.TileSize / 2.0),
                TowerType.Splash => new SplashTower(tile.PixelX + GameMap.TileSize / 2.0,
                                                    tile.PixelY + GameMap.TileSize / 2.0),
                _ => new BasicTower(tile.PixelX + GameMap.TileSize / 2.0,
                                                   tile.PixelY + GameMap.TileSize / 2.0)
            };

            tile.PlaceTower();
            _towers.Add(tower);
            return true;
        }

        public bool TryRemoveTower(double pixelX, double pixelY)
        {
            Tile? tile = _map.GetTileAtPixel(pixelX, pixelY);
            if (tile == null || !tile.IsOccupied) return false;

            Tower? tower = GetTowerAtTile(tile);
            if (tower == null) return false;

            // повертаємо половину вартості
            GameManager.Instance.AddGold(GetCost(GetTowerType(tower)) / 2);

            tile.RemoveTower();
            _towers.Remove(tower);
            return true;
        }

        public void UpdateAll(double deltaTime, List<Enemy> enemies)
        {
            foreach (var tower in _towers)
            {
                tower.AcquireTarget(enemies);

                if (tower is SplashTower splash)
                {
                    if (splash.CurrentTarget != null)
                        splash.AttackSplash(splash.CurrentTarget, enemies, deltaTime);
                }
                else
                {
                    tower.Update(deltaTime);
                }
            }
        }

        public void RenderAll(DrawingContext dc)
        {
            foreach (var tower in _towers)
                tower.Render(dc);
        }

        private Tower? GetTowerAtTile(Tile tile)
        {
            foreach (var tower in _towers)
            {
                Tile? towerTile = _map.GetTileAtPixel(tower.X, tower.Y);
                if (towerTile == tile) return tower;
            }
            return null;
        }

        private static int GetCost(TowerType type) => type switch
        {
            TowerType.Basic => 100,
            TowerType.Sniper => 150,
            TowerType.Splash => 175,
            _ => 100
        };

        private static TowerType GetTowerType(Tower tower) => tower switch
        {
            BasicTower => TowerType.Basic,
            SniperTower => TowerType.Sniper,
            SplashTower => TowerType.Splash,
            _ => TowerType.Basic
        };
    }

    public enum TowerType
    {
        Basic,
        Sniper,
        Splash
    }
}