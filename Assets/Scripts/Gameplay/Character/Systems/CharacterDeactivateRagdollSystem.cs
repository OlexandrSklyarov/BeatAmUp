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
                .Inc<CharacterControllerMovement>()
                .Inc<RagdollState>()
                .Inc<CharacterView>()
                .Inc<CharacterPhysicsBody>()
                .Exc<Death>()
                .Exc<Stun>()
                .End();
            
            var enemies = world
                .Filter<Enemy>()
                .Inc<MovementAI>()
                .Inc<RagdollState>()
                .Inc<CharacterView>()
                .Inc<CharacterPhysicsBody>()
                .Exc<Death>()
                .Exc<Stun>()
                .End();

            var heroPool = world.GetPool<Hero>();
            var enemyPool = world.GetPool<Enemy>();
            var ragdollStatePool = world.GetPool<RagdollState>();
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

                ragdollStatePool.Del(ent);
            }
            
            foreach (var ent in enemies)
            {
                ref var enemy = ref enemyPool.Get(ent);
                ref var body = ref physicsBodyPool.Get(ent);
                ref var view = ref viewPool.Get(ent);
                ref var movementAI = ref aiPool.Get(ent);

                ResetEnemyRagdoll(ref body, ref view, ref movementAI, ref enemy);

                ragdollStatePool.Del(ent);
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
            foreach (var rb in body.BodyRagdoll) 
                rb.isKinematic = true;
            
            var origin = enemy.ViewProvider.BodyHips.position;
            movement.NavAgent.Warp(origin);
            
            view.Animator.enabled = true;
            body.Collider.enabled = true;
        }
    }
}