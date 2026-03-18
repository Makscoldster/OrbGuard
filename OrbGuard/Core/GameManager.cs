using OrbGuard.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrbGuard.Core
{
    public class GameManager
    {
        // Singleton — єдиний екземпляр
        private static GameManager? _instance;
        public static GameManager Instance => _instance ??= new GameManager();

        // Стан гри
        public GamePhase CurrentPhase { get; private set; }
        public int Gold { get; private set; }
        public int CurrentWave { get; private set; }
        public Orb? PlayerOrb { get; private set; }

        // Події — UI підписується і оновлюється автоматично
        public event Action<int>? OnGoldChanged;
        public event Action<GamePhase>? OnPhaseChanged;
        public event Action? OnGameOver;

        private GameManager()
        {
            Gold = 150;
            CurrentWave = 0;
            CurrentPhase = GamePhase.Preparing;
        }

        public void Initialize(Orb orb)
        {
            PlayerOrb = orb;
        }

        public void AddGold(int amount)
        {
            Gold += amount;
            OnGoldChanged?.Invoke(Gold);
        }

        public bool SpendGold(int amount)
        {
            if (Gold < amount) return false; // не вистачає золота
            Gold -= amount;
            OnGoldChanged?.Invoke(Gold);
            return true;
        }

        public void StartWave()
        {
            if (CurrentPhase != GamePhase.Preparing) return;
            CurrentWave++;
            SetPhase(GamePhase.WaveInProgress);
        }

        public void OnWaveComplete()
        {
            SetPhase(GamePhase.Preparing);
        }

        public void TriggerGameOver()
        {
            SetPhase(GamePhase.GameOver);
            OnGameOver?.Invoke();
        }

        public void TriggerVictory()
        {
            SetPhase(GamePhase.Victory);
        }

        private void SetPhase(GamePhase phase)
        {
            CurrentPhase = phase;
            OnPhaseChanged?.Invoke(phase);
        }
    }
}
