using System;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.AI;

namespace BT
{
    public class CreateEnemySystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var entities = world.Filter<CreateEnemyEvent>().End();
            var eventPool = world.GetPool<CreateEnemyEvent>();

            foreach (var e in entities)
            {
                ref var spawnEvent = ref eventPool.Get(e);

                SpawnEnemy(data, world, spawnEvent.Type, spawnEvent.CreatePosition, spawnEvent.CreateRotation);

                eventPool.Del(e);
            }
        }


        private void SpawnEnemy(SharedData data, EcsWorld world, EnemyType type,
            Vector3 createPosition, Quaternion createRotation)
        {
            var enemyView = data.EnemyFactory.GetEnemyView(type);

            var e = world.NewEntity();

            //enemy
            var enemyPool = world.GetPool<EnemyTag>();
            enemyPool.Add(e);

            //view
            var viewPool = world.GetPool<CharacterView>();
            ref var view = ref viewPool.Add(e);
            view.ViewTransform = enemyView.transform;
            view.ViewTransform.SetPositionAndRotation(createPosition, createRotation);
            view.Animator = enemyView.GetComponentInChildren<Animator>();

            //hit
            var hitPool = world.GetPool<HitInteraction>();
            ref var hit = ref hitPool.Add(e);
            hit.HitView = enemyView.GetComponent<IHitReceiver>();
            hit.HitBoxes = enemyView.GetComponentsInChildren<HitBox>();
            Array.ForEach(hit.HitBoxes, h => h.Init());

            //hp
            var hpPool = world.GetPool<Health>();
            ref var hp = ref hpPool.Add(e);
            hp.HP = hp.MaxHP = 100;

            //AI
            var aiPool = world.GetPool<EnemyAI>();
            ref var ai = ref aiPool.Add(e);
            ai.NavAgent = enemyView.GetComponent<NavMeshAgent>();

            //Physics body
            var bodyPool = world.GetPool<CharacterPhysicsBody>();
            ref var comp = ref bodyPool.Add(e);
            comp.Body = enemyView.GetComponent<Rigidbody>();
            comp.Body.isKinematic = true;
        }
    }
}