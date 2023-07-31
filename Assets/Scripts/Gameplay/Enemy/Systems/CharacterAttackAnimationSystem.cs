using Leopotam.EcsLite;

namespace BT
{
    public sealed class CharacterAttackAnimationSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var entities = world
                .Filter<CharacterAttack>()
                .Inc<CharacterView>()
                .Exc<Stun>()                
                .Exc<Death>()                
                .End();

            
            var characterAttackPool = world.GetPool<CharacterAttack>();
            var viewPool = world.GetPool<CharacterView>();

            foreach(var ent in entities)
            {
                ref var attack = ref characterAttackPool.Get(ent);
                ref var view = ref viewPool.Get(ent);

                var attackTrigger = GetAttackTrigger(ref attack);                 

                if (string.IsNullOrEmpty(attackTrigger)) continue;

                view.Animator.SetTrigger(attackTrigger);                     
            }
        }


        private string GetAttackTrigger(ref CharacterAttack attack)
        {        
            if (attack.CurrentPunch != null) return attack.CurrentPunch.StateName;
            if (attack.CurrentKick != null) return attack.CurrentKick.StateName;
            
            return string.Empty;
        }
    }
}