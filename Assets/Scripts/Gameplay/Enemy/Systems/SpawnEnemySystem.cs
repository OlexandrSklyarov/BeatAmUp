using System.Collections.Generic;
using System.Linq;
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
            ref var enemyComp = ref world.GetPool<Enemy>().Add(entity);
            enemyComp.PoolItem = enemyViewProvider;   


            //attackData
            ref var attackData = ref world.GetPool<AttackData>().Add(entity);
            attackData.Data = data.Config.EnemyConfig.EnemyPoolData.First(d => d.Type == type).Attack;


            //attack
            ref var attack = ref world.GetPool<CharacterAttack>().Add(entity);
            attack.IsActiveAttack = false;
            attack.IsNeedFinishAttack = false;
            attack.CurrentPunch = null;
            attack.CurrentKick = null;
            attack.KickQueue = new Queue<CharacterAttackAnimationData>();
            attack.PunchQueue = new Queue<CharacterAttackAnimationData>();
           

            //combat command
            world.GetPool<CombatCommand>().Add(entity);


            //Translation
            var translationPool = world.GetPool<Translation>();
            ref var translation = ref translationPool.Add(entity);
            translation.Value = enemyViewProvider.transform;
            translation.Value.SetPositionAndRotation(createPosition, createRotation);            
                        

            //view
            var viewPool = world.GetPool<CharacterView>();
            ref var view = ref viewPool.Add(entity);
            view.ViewTransform = enemyViewProvider.BodyView;             
            view.Animator = enemyViewProvider.Animator;            
            view.Height = enemyViewProvider.Collider.height;
            view.BodyRadius = enemyViewProvider.Collider.radius;
            view.HipBone = enemyViewProvider.BodyHips;
            view.BodyMaterials = enemyViewProvider.BodyMaterials;


            //hit interaction
            var hitPool = world.GetPool<HitInteraction>();
            ref var hit = ref hitPool.Add(entity);
            hit.HitBoxes = enemyViewProvider.HitBoxes;
            hit.HurtBoxes = enemyViewProvider.HurtBoxes;
           

            //Physics body
            var bodyPool = world.GetPool<CharacterPhysicsBody>();
            ref var body = ref bodyPool.Add(entity);
            
            body.BodyRagdoll = enemyViewProvider.RagdollElements;
            foreach (var r in body.BodyRagdoll) r.isKinematic = true;
            
            var collider = enemyViewProvider.Collider;
            collider.enabled = true;
            body.Collider = collider;

            SetupRagdollBones(enemyViewProvider.BodyHips, data.Config.EnemyConfig, ref body, ref view, ref translation);
           

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


            //Navigation
            var navigationPool = world.GetPool<EnemyNavigation>();
            ref var navigation = ref navigationPool.Add(entity);
            navigation.Destination = Vector3.zero;            
            navigation.StopDistance = ai.NavAgent.radius;            
        }


        private void SetupRagdollBones(Transform hipsBone, EnemyData config, 
            ref CharacterPhysicsBody body, ref CharacterView view, ref Translation translation)
        {
            body.Bones = hipsBone.GetComponentsInChildren<Transform>();

            body.RagdollBoneTransforms = new BoneTransform[body.Bones.Length];        
            body.StandUpFaceBoneTransforms = new BoneTransform[body.Bones.Length];        
            body.StandUpFaceDownBoneTransforms = new BoneTransform[body.Bones.Length]; 

            for(int i = 0; i < body.Bones.Length; i++)
            {       
                body.RagdollBoneTransforms[i] = new BoneTransform();
                body.StandUpFaceBoneTransforms[i] = new BoneTransform();
                body.StandUpFaceDownBoneTransforms[i] = new BoneTransform();
            } 

            //configure stand up bones
            view.Animator.enabled = false;
            var posBefore = translation.Value.position;
            var rotBefore = translation.Value.rotation;

            ConfigureBonesTransform(config.Animation.StandUpFaceUpAnimationName, body.Bones, body.StandUpFaceBoneTransforms, view.Animator);
            ConfigureBonesTransform(config.Animation.StandUpFaceDownAnimationName, body.Bones, body.StandUpFaceDownBoneTransforms, view.Animator);

            translation.Value.position = posBefore;
            translation.Value.rotation = rotBefore;
            view.Animator.enabled = true;
        }

        
        
        private static void ConfigureBonesTransform(string checkName, Transform[] bones, BoneTransform[] targetBones, Animator animator)
        {
            foreach (var clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == checkName)
                {
                    clip.SampleAnimation(animator.gameObject, 0f);
                    GameplayExtensions.PopulateBoneTransforms(bones, targetBones);
                    break;
                }
            }
        }
    }
}