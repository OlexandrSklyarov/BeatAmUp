using System;
using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class InitTestPunchBoxSystem : IEcsInitSystem
    {
        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var punchBoxes = UnityEngine.Object.FindObjectsOfType<TestHitReceiver>();

            Array.ForEach(punchBoxes, box =>
            {
                var e = world.NewEntity();

                //view
                var viewPool =  world.GetPool<CharacterView>();
                ref var view = ref viewPool.Add(e);
                view.ViewTransform = box.transform;
                view.Animator = box.GetComponent<Animator>();
                
                //hit
                var hitPool = world.GetPool<HitInteraction>();
                ref var hit = ref hitPool.Add(e);
                hit.HitView = box.GetComponent<IHitReceiver>();
                hit.HitBoxes = box.GetComponentsInChildren<HitBox>();
                Array.ForEach(hit.HitBoxes, h => h.Init());

                //hp
                var hpPool =  world.GetPool<Health>();
                ref var hp = ref hpPool.Add(e);
                hp.HP = hp.MaxHP = 100;
            });
                        
        }
    }
}