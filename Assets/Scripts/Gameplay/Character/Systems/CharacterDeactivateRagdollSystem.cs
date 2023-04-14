using Leopotam.EcsLite;
using UnityEngine;

namespace BT
{
    public class CharacterDeactivateRagdollSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            
            var heroes = world
                .Filter<Hero>()
                .Inc<DeactivateRagdollEvent>()
                .Inc<CharacterControllerMovement>()
                .Inc<RagdollState>()
                .Inc<CharacterView>()
                .Inc<CharacterPhysicsBody>()
                .Exc<Death>()
                .End();
            
            var enemies = world
                .Filter<Enemy>()
                .Inc<DeactivateRagdollEvent>()
                .Inc<MovementAI>()
                .Inc<RagdollState>()
                .Inc<CharacterView>()
                .Inc<CharacterPhysicsBody>()
                .Exc<Death>()
                .End();

            var heroPool = world.GetPool<Hero>();
            var enemyPool = world.GetPool<Enemy>();
            var ragdollEventPool = world.GetPool<DeactivateRagdollEvent>();
            var ragdollStatePoll = world.GetPool<RagdollState>();
            var physicsBodyPool = world.GetPool<CharacterPhysicsBody>();
            var viewPool = world.GetPool<CharacterView>();
            var aiPool = world.GetPool<MovementAI>();
            var characterControllerMovementPool = world.GetPool<CharacterControllerMovement>();
            
            foreach (var ent in heroes)
            {
                ref var hero = ref heroPool.Get(ent);
                ref var movement = ref characterControllerMovementPool.Get(ent);
                ref var body = ref physicsBodyPool.Get(ent);
                ref var view = ref viewPool.Get(ent);

                ResetHeroRagdoll(ref body, ref view, ref movement, ref hero);

                ragdollEventPool.Del(ent);
                ragdollStatePoll.Del(ent);
            }
            
            foreach (var ent in enemies)
            {
                ref var enemy = ref enemyPool.Get(ent);
                ref var body = ref physicsBodyPool.Get(ent);
                ref var view = ref viewPool.Get(ent);
                ref var movementAI = ref aiPool.Get(ent);

                ResetEnemyRagdoll(ref body, ref view, ref movementAI, ref enemy);

                ragdollEventPool.Del(ent);
                ragdollStatePoll.Del(ent);
            }
        }

        
        private void ResetHeroRagdoll(ref CharacterPhysicsBody body, ref CharacterView view,
            ref CharacterControllerMovement movement, ref Hero hero)
        {
            Util.Debug.PrintColor($"Disable hero ragdoll hips pos {hero.ViewProvider.BodyHips.position}", Color.yellow);
        }


        private void ResetEnemyRagdoll(ref CharacterPhysicsBody body, ref CharacterView view, 
            ref MovementAI movement, ref Enemy enemy)
        {
            var origin = enemy.ViewProvider.BodyHips.position;
            movement.NavAgent.Warp(origin);
                
            foreach (var rb in body.BodyRagdoll)
            {
                rb.isKinematic = true;
            }
            
            view.Animator.enabled = true;
        }
    }
}