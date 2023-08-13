using Gameplay.FX;
using Services.GameSettings;
using Services.Input;

namespace BT
{
    public class SharedData
    {
        public GameConfig Config;
        public WorldData WorldData;
        public VisualFXController VFXController;
        public EnemyFactory EnemyFactory;
        public CheckCollisionServices CollisionService;
        public PlayerBindInputService PlayerInputService;
        public GameSettingsService GameSettings;
    }
}