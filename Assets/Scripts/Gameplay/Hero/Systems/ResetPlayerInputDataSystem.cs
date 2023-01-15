using Leopotam.EcsLite;

namespace BT
{
    public sealed class ResetPlayerInputDataSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var control = systems.GetShared<SharedData>().InputProvider;

            var entities = systems.GetWorld()
                .Filter<HeroAttack>()
                .Inc<HeroTag>()
                .End();

            var attackPool = systems.GetWorld().GetPool<HeroAttack>();

            foreach(var e in entities)
            {
                control.ResetInput();

                ref var attack = ref attackPool.Get(e);
                attack.CurrentKick = null;
                attack.CurrentPunch = null;
            }
        }
    }
}