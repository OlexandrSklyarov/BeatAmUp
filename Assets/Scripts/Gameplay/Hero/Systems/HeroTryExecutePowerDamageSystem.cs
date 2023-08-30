using Leopotam.EcsLite;

namespace BT
{
    public class HeroTryExecutePowerDamageSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var heroes = world
                .Filter<Hero>()
                .Inc<CombatCommand>()
                .Inc<CharacterGrounded>()
                .Inc<CharacterAttack>()
                .Exc<Stun>()
                .Exc<Death>()
                .End();           

            var combatCommandPool = world.GetPool<CombatCommand>();
            var attackPool = world.GetPool<CharacterAttack>();

            foreach (var heroEnt in heroes)
            {
                ref var combat = ref combatCommandPool.Get(heroEnt);
                ref var attack = ref attackPool.Get(heroEnt);

                if (!combat.IsPunch && !combat.IsKick) return;
                TryExecutePowerDamage(ref attack);             
            }
        }


        private void TryExecutePowerDamage(ref CharacterAttack attack)
        {
            if (attack.HitCount < ConstPrm.Character.MAX_HIT_COUNT) return;

            attack.HitCount = 0;
            attack.HitResetTimer = 0f;
            attack.IsPowerfulDamage = true;
        }
    }
}
