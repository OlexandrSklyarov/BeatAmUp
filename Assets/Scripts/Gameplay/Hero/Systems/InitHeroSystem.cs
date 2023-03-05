using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public sealed class InitHeroSystem : IEcsInitSystem
    {
        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            var heroGO = UnityEngine.Object.Instantiate
            (
                data.Config.PlayerData.Prefab, 
                data.WorldData.HeroSpawnPoint.position, 
                Quaternion.identity
            );

            var heroEntity = world.NewEntity();

            //hero
            var heroTagPool =  world.GetPool<HeroTag>();
            heroTagPool.Add(heroEntity);

            //input
            var inputDataPool =  world.GetPool<CharacterCommand>();
            inputDataPool.Add(heroEntity);

            //movement
            var movementPool =  world.GetPool<Movement>();
            ref var movement = ref movementPool.Add(heroEntity);
            var characterController = heroGO.GetComponent<CharacterController>();
            movement.characterController = characterController;
            movement.Transform = heroGO.transform;   

            //view
            var viewPool = world.GetPool<CharacterView>();
            ref var view = ref viewPool.Add(heroEntity);
            view.Animator = heroGO.GetComponentInChildren<Animator>();
            view.ViewTransform = heroGO.transform.GetChild(0).transform; 
            view.Height = characterController.height;
            view.BodyRadius = characterController.radius;
          
            //hit interaction
            var hitPool = world.GetPool<HitInteraction>();
            ref var hit = ref hitPool.Add(heroEntity);
            hit.HitView = heroGO.GetComponent<IHitReceiver>();
            hit.HitBoxes = heroGO.GetComponentsInChildren<HitBox>();
            Array.ForEach(hit.HitBoxes, h => h.Init());
            

           //attack
            var heroHandleAttackPool = world.GetPool<HeroAttack>();
            ref var heroAttack = ref heroHandleAttackPool.Add(heroEntity);            
            heroAttack.IsActiveAttack = false;    
            heroAttack.IsNeedFinishAttack = false;
            heroAttack.CurrentPunch = null; 
            heroAttack.CurrentKick = null; 
            heroAttack.KickQueue = new Queue<HeroAttackAnimationData>();
            heroAttack.PunchQueue = new Queue<HeroAttackAnimationData>();
            heroAttack.PunchData = data.Config.HeroAttackData.PunchAnimationData;
            heroAttack.KickData = data.Config.HeroAttackData.KickAnimationData;
            heroAttack.PunchFinishData = data.Config.HeroAttackData.PunchAnimationFinishData;
            heroAttack.KickFinishData = data.Config.HeroAttackData.KickAnimationFinishData;
            heroAttack.LastTargetHP = 100;  

            //HP
            var healthPool = world.GetPool<Health>();
            ref var heroHealth = ref healthPool.Add(heroEntity); 
            heroHealth.HP = data.Config.PlayerData.StartHP;
            heroHealth.MaxHP = data.Config.PlayerData.StartHP;            

            Util.Debug.Print($"hero init...");
        }
    }
}