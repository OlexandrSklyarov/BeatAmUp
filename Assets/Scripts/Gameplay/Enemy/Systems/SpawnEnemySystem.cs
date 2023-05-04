using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace BT
{
    public sealed class SpawnEnemySystem : IEcsRunSystem
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
            enemyComp.PoolItem = enemyViewProvider;   


            //Translation
            var translationPool = world.GetPool<Translation>();
            ref var translation = ref translationPool.Add(entity);
            translation.Value = enemyViewProvider.transform;
            translation.Value.SetPositionAndRotation(createPosition, createRotation);            
                        

            //view
            var viewPool = world.GetPool<CharacterView>();
            ref var view = ref viewPool.Add(entity);
            view.ViewTransform = enemyViewProvider.transform.GetChild(0).transform;             
            view.Animator = enemyViewProvider.GetComponentInChildren<Animator>();            
            view.Height = enemyViewProvider.Collider.height;
            view.BodyRadius = enemyViewProvider.Collider.radius;
            view.HipBone = enemyViewProvider.BodyHips;
           

            //Physics body
            var bodyPool = world.GetPool<CharacterPhysicsBody>();
            ref var body = ref bodyPool.Add(entity);
            
            body.BodyRagdoll = enemyViewProvider.RagdollElements;
            foreach (var r in body.BodyRagdoll) r.isKinematic = true;
            
            var collider = enemyViewProvider.Collider;
            collider.enabled = true;
            body.Collider = collider;

            SetupRagdollBones(enemyViewProvider.BodyHips, data.Config.EnemyConfig, ref body, ref view, ref translation);


            //hit
            var hitPool = world.GetPool<HitInteraction>();
            ref var hit = ref hitPool.Add(entity);
            hit.HitBoxes = enemyViewProvider.GetComponentsInChildren<HitBox>();
            
            hit.HurtBoxes = enemyViewProvider.GetComponentsInChildren<HurtBox>();
            foreach(var h in hit.HurtBoxes) h.Init();


            //hp
            var hpPool = world.GetPool<Health>();
            ref var hpComp = ref hpPool.Add(entity);
            hpComp.CurrentHP = hpComp.MaxHP = 100;


            //AI
            var aiPool = world.GetPool<MovementAI>();
            ref var ai = ref aiPool.Add(entity);
            ai.NavAgent = enemyViewProvider.GetComponent<NavMeshAgent>();
            ai.NavAgent.Warp(createPosition);
            ai.NavAgent.avoidancePriority = 50 + Random.Range(0, 45);
            ai.NavAgent.speed = data.Config.EnemyConfig.Movement.Speed;   
            ai.NavAgent.acceleration = data.Config.EnemyConfig.Movement.Acceleration;   
            ai.NavAgent.angularSpeed = data.Config.EnemyConfig.Movement.AngularSpeed;   
            ai.NavAgent.updateRotation = false;               
        }


        private void SetupRagdollBones(Transform hipsBone, EnemyData config, 
            ref CharacterPhysicsBody body, ref CharacterView view, ref Translation translation)
        {
            body.Bones = hipsBone.GetComponentsInChildren<Transform>();

            body.RagdollBoneTransforms = new BoneTransform[body.Bones.Length];        
            body.StandUpBoneTransforms = new BoneTransform[body.Bones.Length]; 

            for(int i = 0; i < body.Bones.Length; i++)
            {
                body.RagdollBoneTransforms[i] = new BoneTransform();        
                body.StandUpBoneTransforms[i] = new BoneTransform();
            } 

            //configure stand up bones
            var posBefore = translation.Value.position;
            var rotBefore = translation.Value.rotation;

            foreach(var clip in view.Animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == config.Animation.StandUpFaceUpAnimationName)
                {
                    clip.SampleAnimation(view.Animator.gameObject, 0f);
                    GameplayExtensions.PopulateBoneTransforms(body.Bones, body.StandUpBoneTransforms);
                    break;
                }
            }

            translation.Value.position = posBefore;
            translation.Value.rotation = rotBefore;
        }
    }
}