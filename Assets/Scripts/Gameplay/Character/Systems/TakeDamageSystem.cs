using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class TakeDamageSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var filter = world
                .Filter<TakeDamageEvent>()
                .Inc<Health>()
                .Inc<CharacterView>()
                .End();
            
            var damageEventPool = world.GetPool<TakeDamageEvent>();
            var viewPool = world.GetPool<CharacterView>();
            var hpPool = world.GetPool<Health>();

            foreach (var e in filter)
            {
                ref var hpComp = ref hpPool.Get(e);
                ref var view = ref viewPool.Get(e);
                ref var damageEvent = ref damageEventPool.Get(e);

                var prev = hpComp.HP;
                hpComp.HP = Mathf.Max(0, hpComp.HP - damageEvent.DamageAmount); 
                view.HitView.Hit();
                Util.Debug.PrintColor($"Add hit {view.HitView} damage {damageEvent.DamageAmount} hp {prev}/{hpComp.HP}", Color.yellow);                 
                
                damageEventPool.Del(e);                
            }
        }
    }
}