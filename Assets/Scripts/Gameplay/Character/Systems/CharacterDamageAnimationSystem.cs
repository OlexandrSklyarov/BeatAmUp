using Leopotam.EcsLite;

namespace BT
{
    public sealed class CharacterDamageAnimationSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var entities = world
                .Filter<TakeDamageEvent>()
                .Inc<CharacterView>()
                .Exc<RagdollState>()
                .End();

            var damageEventPool = world.GetPool<TakeDamageEvent>();
            var viewPool = world.GetPool<CharacterView>();
            var deathPool = world.GetPool<Death>();

            foreach (var ent in entities)
            {
                ref var damageEvt = ref damageEventPool.Get(ent);
                ref var view = ref viewPool.Get(ent);

                if (deathPool.Has(ent))
                {
                    DeathAnimation(ref damageEvt, ref view);
                    continue;
                }

                DamageAnimation(ref damageEvt, ref view);                
            }
        }
        

        private void DeathAnimation(ref TakeDamageEvent damageEvt, ref CharacterView view)
        {
            if (damageEvt.IsHammeringDamage)
            {
                PlayHammeringDamage(ref view);
            }
            else
            {
                SetDeath(ref view);
            }
        }

        private void SetDeath(ref CharacterView view) => view.Animator.SetBool(ConstPrm.Animation.DEATH, true);


        private void DamageAnimation(ref TakeDamageEvent damageEvt, ref CharacterView view)
        {
            if (view.Animator.IsCurrentAnimationState(ConstPrm.Animation.HAMMERING_DAMAGE)) return;
            
            if (damageEvt.IsHammeringDamage)
            {
                PlayHammeringDamage(ref view);
            }
            else
            {
                var zoneIndex = (damageEvt.IsTopBodyDamage) ? 0 : 1;              
                PlaySimpleDamage(ref view, zoneIndex);
            }
        }
        

        private void PlayHammeringDamage(ref CharacterView view)
        {
            view.Animator.SetTrigger(ConstPrm.Animation.HAMMERING_DAMAGE);
        }


        private void PlaySimpleDamage(ref CharacterView view, int damageZoneIndex)
        {
            view.Animator.SetInteger(ConstPrm.Animation.DAMAGE_TYPE, damageZoneIndex);
            view.Animator.SetTrigger(ConstPrm.Animation.DAMAGE);
        }
    }
}