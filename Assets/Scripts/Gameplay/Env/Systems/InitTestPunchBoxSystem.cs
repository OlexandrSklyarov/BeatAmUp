using System;
using System.Collections.Generic;
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
                var boxEntity = world.NewEntity();

                //view
                var viewPool =  world.GetPool<CharacterView>();
                ref var view = ref viewPool.Add(boxEntity);
                view.ViewTransform = box.transform;
                view.Animator = box.GetComponent<Animator>();
                view.HitView = box.GetComponent<IHitReceiver>();

                //hp
                var hpPool =  world.GetPool<Health>();
                ref var hp = ref hpPool.Add(boxEntity);
                hp.HP = hp.MaxHP = 100;
            });
                        
        }
    }
}