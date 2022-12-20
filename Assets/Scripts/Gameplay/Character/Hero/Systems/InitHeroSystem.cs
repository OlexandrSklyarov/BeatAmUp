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
            heroAttack.CurrentPunchState = PunchState.NONE; 
            heroAttack.CurrentKickState = KickState.NONE; 
            heroAttack.KickQueue = new Queue<KickState>();
            heroAttack.PunchQueue = new Queue<PunchState>();
            
            Util.Debug.Print($"hero init...");
        }
    }
}