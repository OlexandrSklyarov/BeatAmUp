using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.InputSystem.Users;

namespace BT
{
    public sealed class SpawnHeroSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var data = systems.GetShared<SharedData>();

            ClearCreateHeroEvents(world);

            var entities = world.Filter<CreateHeroRequest>().End();
            var requestPool = world.GetPool<CreateHeroRequest>();

            foreach (var e in entities)
            {
                ref var request = ref requestPool.Get(e);

                Spawn(world, data, ref request);
                SendSpawnHeroEvent(world, ref request);

                requestPool.Del(e);
            }
        }


        private void ClearCreateHeroEvents(EcsWorld world)
        {
            var filter = world.Filter<SpawnedHeroEvent>().End();
            var eventPool = world.GetPool<SpawnedHeroEvent>();

            foreach (var ent in filter)
            {
                eventPool.Del(ent);
            }
        }


        private void SendSpawnHeroEvent(EcsWorld world, ref CreateHeroRequest spawnRequest)
        {
            var entity = world.NewEntity();
            var eventPool = world.GetPool<SpawnedHeroEvent>();
            ref var evt = ref eventPool.Add(entity);
            evt.NewHeroID = spawnRequest.SpawnIndex;
        }


        private void Spawn(EcsWorld world, SharedData data, ref CreateHeroRequest spawnRequest)
        {
            var monoView = InstantiateHeroView(data, ref spawnRequest);

            SetupEntity(world, ref spawnRequest, monoView);
        }


        private IHeroViewProvider InstantiateHeroView(SharedData data, ref CreateHeroRequest spawnRequest)
        {
            var heroView = UnityEngine.Object.Instantiate
            (
                spawnRequest.UnitData.Prefab,
                data.WorldData.HeroSpawnPoints[spawnRequest.SpawnIndex].position,
                Quaternion.identity
            );

            heroView.Init();

            return heroView;
        }


        private void SetupEntity(EcsWorld world, ref CreateHeroRequest spawnRequest, IHeroViewProvider monoView)
        {
            var entity = world.NewEntity();

            //hero
            ref var hero = ref world.GetPool<Hero>().Add(entity);
            hero.ID = spawnRequest.SpawnIndex;


            //input command
            world.GetPool<MovementCommand>().Add(entity);


            //combat command
            world.GetPool<CombatCommand>().Add(entity);


            //movement
            var movementPool = world.GetPool<CharacterControllerMovement>();
            ref var movement = ref movementPool.Add(entity);
            var characterController = monoView.CC;
            movement.CharacterController = characterController;


            //translation
            var translationPool = world.GetPool<Translation>();
            ref var translation = ref translationPool.Add(entity);
            translation.Value = monoView.Transform;


            //view
            var viewPool = world.GetPool<CharacterView>();
            ref var view = ref viewPool.Add(entity);
            view.Animator = monoView.Animator;
            view.ViewTransform = monoView.Transform.GetChild(0).transform;
            view.Height = characterController.height;
            view.BodyRadius = characterController.radius;
            view.HipBone = monoView.BodyHips;


            //hit interaction
            var hitPool = world.GetPool<HitInteraction>();
            ref var hit = ref hitPool.Add(entity);
            hit.HitBoxes = monoView.HitBoxes;
            hit.HurtBoxes = monoView.HurtBoxes;


            //attackData
            ref var attackData = ref world.GetPool<AttackData>().Add(entity);
            attackData.Data = spawnRequest.UnitData.Attack;


            //attack
            var heroHandleAttackPool = world.GetPool<CharacterAttack>();
            ref var heroAttack = ref heroHandleAttackPool.Add(entity);
            heroAttack.IsActiveAttack = false;
            heroAttack.IsNeedFinishAttack = false;
            heroAttack.CurrentPunch = null;
            heroAttack.CurrentKick = null;
            heroAttack.KickQueue = new Queue<CharacterAttackAnimationData>();
            heroAttack.PunchQueue = new Queue<CharacterAttackAnimationData>();


            //HP
            var healthPool = world.GetPool<Health>();
            ref var heroHealth = ref healthPool.Add(entity);
            heroHealth.CurrentHP = heroHealth.MaxHP = spawnRequest.UnitData.Data.StartHP;


            //input
            var heroInputPool = world.GetPool<HeroInputUser>();
            ref var input = ref heroInputPool.Add(entity);
            var action = new InputServices();
            input.InputProvider = new InputHandleProvider(action);
            input.InputProvider.Enable();
            input.Device = spawnRequest.Device;
            input.User = InputUser.PerformPairingWithDevice(spawnRequest.Device);

            BindDeviceToUser(action, ref input);

            Util.Debug.PrintColor($"Hero init: {input.Device.name}", UnityEngine.Color.green);
        }


        private void BindDeviceToUser(InputServices action, ref HeroInputUser inputUser)
        {
            var deviceName = inputUser.Device.name;
            inputUser.User.ActivateControlScheme(action.controlSchemes.First(s => s.name.Equals(deviceName)));
            inputUser.User.AssociateActionsWithUser(action);
        }
    }
}