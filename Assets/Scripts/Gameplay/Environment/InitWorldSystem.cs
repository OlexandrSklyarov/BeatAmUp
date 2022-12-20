using Leopotam.EcsLite;

namespace BT
{
    public sealed class InitWorldSystem : IEcsInitSystem
    {
        public void Init(IEcsSystems systems)
        {
            Util.Debug.Print("Init world...");
        }
    }
}