
using Gameplay.FX;

namespace BT
{
    public class SharedData
    {
        public InputHandleProvider InputProvider;
        public GameConfig Config;
        public WorldData WorldData;
        public VisualFXController VFXController;
        public EnemyFactory EnemyFactory;
        public CheckCollisionServices CollisionService;
    }
}