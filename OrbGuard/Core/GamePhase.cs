using System;
using System.Collections.Generic;
using System.Text;

namespace OrbGuard.Core
{
    public enum GamePhase
    {
        Preparing,       // між хвилями, гравець будує башти
        WaveInProgress,  // хвиля йде, вороги на полі
        GameOver,        // орб знищено
        Victory          // всі хвилі пройдено
    }
}
