using Leopotam.EcsLite;

namespace Gameplay.Character.Hero
{
    public sealed class ResetPlayerInputDataSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var entities = systems.GetWorld()
                .Filter<CharacterCommand>()
                .Inc<HeroAttack>()
                .Inc<HeroTag>()
                .End();

            var inputDataPool = systems.GetWorld().GetPool<CharacterCommand>();
            var attackPool = systems.GetWorld().GetPool<HeroAttack>();

            foreach(var e in entities)
            {
                ref var input = ref inputDataPool.Get(e); 
                input.IsJump = false;
                input.IsKick = false;
                input.IsPunch = false;

                ref var attack = ref attackPool.Get(e);
                attack.CurrentKickState = KickState.NONE;
                attack.CurrentPunchState = PunchState.NONE;
            }
        }
    }
}