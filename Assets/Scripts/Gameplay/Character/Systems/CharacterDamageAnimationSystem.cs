using Leopotam.EcsLite;

namespace BT
{
    public sealed class CharacterDamageAnimationSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var entities = world
                .Filter<DamageView>()
                .Inc<CharacterView>()
                .Inc<CharacterPhysicsBody>()
                .End();

            var damageViewPool = world.GetPool<DamageView>();
            var viewPool = world.GetPool<CharacterView>();
            var bodyPool = world.GetPool<CharacterPhysicsBody>();

            foreach (var e in entities)
            {
                ref var damageView = ref damageViewPool.Get(e);
                ref var view = ref viewPool.Get(e);
                ref var body = ref bodyPool.Get(e);

                if (damageView.IsFinalDamage)
                {
                    if (damageView.IsHammeringDamage)
                        view.Animator.SetTrigger(ConstPrm.Animation.HAMMERING_DAMAGE);
                    else
                        view.Animator.SetTrigger(ConstPrm.Animation.DEATH);

                    body.Collider.enabled = false;
                }
                else
                {                    
                    var damageType = (damageView.IsTopBodyDamage) ? 0 : 1;
                    view.Animator.SetInteger(ConstPrm.Animation.DAMAGE_TYPE, damageType);
                    view.Animator.SetTrigger(ConstPrm.Animation.DAMAGE);                    
                }

                damageViewPool.Del(e);
            }
        }
    }
}