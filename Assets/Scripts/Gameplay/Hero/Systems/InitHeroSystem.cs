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

            var heroGO = Object.Instantiate
            (
                data.Config.PlayerData.Prefab, 
                data.WorldData.HeroSpawnPoint.position, 
                Quaternion.identity
            );

            var heroEntity = world.NewEntity();

            
            var heroTagPool =  world.GetPool<HeroTag>();
            heroTagPool.Add(heroEntity);


            var inputDataPool =  world.GetPool<CharacterCommand>();
            inputDataPool.Add(heroEntity);


            var viewPool = world.GetPool<CharacterView>();
            ref var view = ref viewPool.Add(heroEntity);
            view.Animator = heroGO.GetComponentInChildren<Animator>();
            view.ViewTransform = heroGO.transform.GetChild(0).transform;  


            var movementPool =  world.GetPool<Movement>();
            ref var movement = ref movementPool.Add(heroEntity);
            movement.characterController = heroGO.GetComponent<CharacterController>();
            movement.Transform = heroGO.transform;   

           
            var heroHandleAttackPool = world.GetPool<HeroAttack>();
            ref var heroAttack = ref heroHandleAttackPool.Add(heroEntity);            
            heroAttack.IsActiveAttack = false;    
            heroAttack.IsNeedFinishAttack = false;
            heroAttack.CurrentPunch = null; 
            heroAttack.CurrentKick = null; 
            heroAttack.KickQueue = new Queue<HeroAttackAnimationData>();
            heroAttack.PunchQueue = new Queue<HeroAttackAnimationData>();
            heroAttack.PunchData = data.Config.PlayerData.PunchAnimationData;
            heroAttack.KickData = data.Config.PlayerData.KickAnimationData;
            heroAttack.PunchFinishData = data.Config.PlayerData.PunchAnimationFinishData;
            heroAttack.KickFinishData = data.Config.PlayerData.KickAnimationFinishData;
            
            Util.Debug.Print($"hero init...");
        }
    }
}