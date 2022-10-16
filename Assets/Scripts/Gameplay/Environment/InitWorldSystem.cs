using Leopotam.EcsLite;

namespace Gameplay.Environment
{
    public sealed class InitWorldSystem : IEcsInitSystem
    {
        public void Init(IEcsSystems systems)
        {
            Util.Debug.Print("Init world...");
        }
    }
}