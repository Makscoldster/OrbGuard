// UI/MainWindow.xaml.cs
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using OrbGuard.Core;
using OrbGuard.Entities;
using OrbGuard.Managers;
using OrbGuard.Map;
using OrbGuard.Systems;

namespace OrbGuard.UI
{
    public partial class MainWindow : Window
    {
        // Core
        private GameLoop _gameLoop;
        private GameMap _gameMap;

        // Managers
        private EnemyManager _enemyManager;
        private TowerManager _towerManager;
        private WaveManager _waveManager;

        // Systems
        private CollisionSystem _collisionSystem;

        // Entities
        private Orb _orb;

        // Рендер
        private DrawingGroup _drawingGroup;
        private DrawingImage _drawingImage;

        // Константи
        private const int TotalWaves = 10;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
            SubscribeToEvents();
        }

        private void InitializeGame()
        {
            _gameMap = new GameMap();

            double orbX = GameMap.TileSize * 20 + GameMap.TileSize / 2.0;
            double orbY = GameMap.TileSize * 10 + GameMap.TileSize / 2.0;
            _orb = new Orb(orbX, orbY);

            // managers
            _enemyManager = new EnemyManager(_gameMap);
            _towerManager = new TowerManager(_gameMap);
            _waveManager = new WaveManager(_enemyManager);
            _collisionSystem = new CollisionSystem();

            // GameManager
            GameManager.Instance.Initialize(_orb);
            SubscribeToGameManagerEvents();

            _drawingGroup = new DrawingGroup();
            _drawingImage = new DrawingImage(_drawingGroup);
            var imageSource = new System.Windows.Controls.Image
            {
                Source = _drawingImage,
                Width = GameMap.Cols * GameMap.TileSize,
                Height = GameMap.Rows * GameMap.TileSize
            };
            GameCanvas.Children.Add(imageSource);

            // Game loop
            _gameLoop = new GameLoop();
            _gameLoop.OnUpdate += Update;
            _gameLoop.OnRender += Render;
            _gameLoop.Start();
        }

        private void SubscribeToEvents()
        {
            Closing += (s, e) => _gameLoop.Stop();
        }

        private void SubscribeToGameManagerEvents()
        {
            GameManager.Instance.OnGoldChanged += gold => GoldText.Text = gold.ToString();
            GameManager.Instance.OnPhaseChanged += phase =>
            {
                StartWaveBtn.IsEnabled = phase == GamePhase.Preparing;
                WaveText.Text = $"{GameManager.Instance.CurrentWave} / {TotalWaves}";
            };
            GameManager.Instance.OnGameOver += () =>
            {
                _gameLoop.Stop();

                this.IsEnabled = false;

                MessageBox.Show("Орб знищено! Гра закінчена.", "Game Over",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                Application.Current.Shutdown();
            };
        }

        private void Update(double deltaTime)
        {
            if (GameManager.Instance.CurrentPhase != GamePhase.WaveInProgress) return;

            _waveManager.Update(deltaTime);
            _enemyManager.UpdateAll(deltaTime);
            _towerManager.UpdateAll(deltaTime, (System.Collections.Generic.List<OrbGuard.Entities.Enemies.Enemy>)_enemyManager.Enemies);
            _orb.Update(deltaTime);

            _collisionSystem.CheckEnemiesAtOrb(
                (System.Collections.Generic.List<OrbGuard.Entities.Enemies.Enemy>)_enemyManager.Enemies,
                _orb);

            _collisionSystem.CheckVictory(TotalWaves);

            UpdateOrbHud();

            if (_waveManager.IsWaveComplete)
                GameManager.Instance.OnWaveComplete();
        }

        private void Render()
        {
            using var context = _drawingGroup.Open();
            _gameMap.Render(context);
            _enemyManager.RenderAll(context);
            _towerManager.RenderAll(context);
            _orb.Render(context);
        }

        private void UpdateOrbHud()
        {
            Dispatcher.Invoke(() =>
            {
                OrbHpText.Text = $"{(int)_orb.CurrentHp} / {(int)_orb.MaxHp}";
                double ratio = _orb.CurrentHp / _orb.MaxHp;
                OrbHpBar.Width = 176 * ratio;
            });
        }

        // Події миші
        private void GameCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            Point pos = e.GetPosition(GameCanvas);
            TowerType type = GetSelectedTowerType();
            _towerManager.TryPlaceTower(type, pos.X, pos.Y);
        }

        private void GameCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            Point pos = e.GetPosition(GameCanvas);
            _towerManager.TryRemoveTower(pos.X, pos.Y);
        }

        private TowerType GetSelectedTowerType()
        {
            if (SniperTowerBtn.IsChecked == true) return TowerType.Sniper;
            if (SplashTowerBtn.IsChecked == true) return TowerType.Splash;
            return TowerType.Basic;
        }

        // Кнопки
        private void StartWaveBtn_Click(object sender, RoutedEventArgs e)
        {
            _waveManager.StartWave(GameManager.Instance.CurrentWave + 1);
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_gameLoop.IsRunning)
            {
                _gameLoop.Pause();
                PauseBtn.Content = "▶ Продовжити";
            }
            else
            {
                _gameLoop.Resume();
                PauseBtn.Content = "⏸ Пауза";
            }
        }
    }
}