using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;


namespace OrbGuard.Core
{
    public class GameLoop
    {
        private readonly DispatcherTimer _timer;
        private DateTime _lastUpdate;

        public bool IsRunning { get; private set; }

        // Події на які підписуються GameManager, Managers, Renderer
        public event Action<double>? OnUpdate; // deltaTime в секундах
        public event Action? OnRender;

        public GameLoop()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16) // ~60 FPS
            };
            _timer.Tick += Tick;
        }

        public void Start()
        {
            if (IsRunning) return;
            _lastUpdate = DateTime.Now;
            IsRunning = true;
            _timer.Start();
        }

        public void Stop()
        {
            IsRunning = false;
            _timer.Stop();
        }

        public void Pause()
        {
            IsRunning = false;
            _timer.Stop();
        }

        public void Resume()
        {
            if (IsRunning) return;
            _lastUpdate = DateTime.Now; // скидаємо час щоб уникнути стрибка deltaTime
            IsRunning = true;
            _timer.Start();
        }

        private void Tick(object? sender, EventArgs e)
        {
            var now = DateTime.Now;
            double deltaTime = (now - _lastUpdate).TotalSeconds;
            _lastUpdate = now;

            // Захист від великого deltaTime (наприклад якщо вікно було згорнуте)
            deltaTime = Math.Min(deltaTime, 0.1);

            OnUpdate?.Invoke(deltaTime);
            OnRender?.Invoke();
        }
    }
}
