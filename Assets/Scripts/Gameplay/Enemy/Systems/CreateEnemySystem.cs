using System;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.AI;

namespace BT
{
    public sealed class CreateEnemySystem : IEcsRunSystem
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
            var enemyViewProvider = data.EnemyFactory.GetEnemyView(type);

            var entity = world.NewEntity();

            //enemy
            var enemyPool = world.GetPool<Enemy>();
            ref var enemyComp = ref enemyPool.Add(entity);
            enemyComp.ViewProvider = enemyViewProvider;

            //Physics body
            var bodyPool = world.GetPool<CharacterPhysicsBody>();
            ref var comp = ref bodyPool.Add(entity);
            comp.Body = enemyViewProvider.GetComponent<Rigidbody>();
            comp.Body.isKinematic = true;
            var collider = enemyViewProvider.GetComponent<CapsuleCollider>();
            collider.enabled = true;
            comp.Collider = collider;

            //view
            var viewPool = world.GetPool<CharacterView>();
            ref var view = ref viewPool.Add(entity);
            view.ViewTransform = enemyViewProvider.transform.GetChild(0).transform;             
            view.Animator = enemyViewProvider.GetComponentInChildren<Animator>();            
            view.Height = collider.height;
            view.BodyRadius = collider.radius;

            //hit
            var hitPool = world.GetPool<HitInteraction>();
            ref var hit = ref hitPool.Add(entity);
            hit.HitView = enemyViewProvider.GetComponent<IHitReceiver>();
            hit.HitBoxes = enemyViewProvider.GetComponentsInChildren<HitBox>();
            Array.ForEach(hit.HitBoxes, h => h.Init());


            //hp
            var hpPool = world.GetPool<Health>();
            ref var hpComp = ref hpPool.Add(entity);
            hpComp.HP = hpComp.MaxHP = 100;

            //AI
            var aiPool = world.GetPool<MovementAI>();
            ref var ai = ref aiPool.Add(entity);
            ai.NavAgent = enemyViewProvider.GetComponent<NavMeshAgent>();
            ai.NavAgent.Warp(createPosition);  
            ai.NavAgent.updateRotation = false;
            ai.NavAgent.speed = data.Config.EnemyConfig.Movement.Speed;   
            ai.NavAgent.angularSpeed = data.Config.EnemyConfig.Movement.AngularSpeed;   
            ai.MyTransform = enemyViewProvider.transform;
            ai.MyTransform.SetPositionAndRotation(createPosition, createRotation);   
        }
    }
}