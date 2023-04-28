using Leopotam.EcsLite;

namespace BT
{
    public sealed class EnemyCharacterDeactivateRagdollSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
                
            var enemies = world
                .Filter<Enemy>()
                .Inc<RagdollState>()
                .Inc<MovementAI>()
                .Inc<CharacterView>()
                .Inc<CharacterPhysicsBody>()
                .Exc<Stun>()
                .Exc<Death>()
                .End();

            var ragdollStatePool = world.GetPool<RagdollState>();
            var viewPool = world.GetPool<CharacterView>();
            var bodyPool = world.GetPool<CharacterPhysicsBody>();
            var movementAiPool = world.GetPool<MovementAI>();
            var deathPool = world.GetPool<Death>();
            var standUpAnimationPool = world.GetPool<StandUpAnimationEvent>();

            foreach (var ent in enemies)
            {
                ref var view = ref viewPool.Get(ent);
                ref var body = ref bodyPool.Get(ent);
                ref var movementAI = ref movementAiPool.Get(ent);
                 
                var isGrounded = TryResetRagdoll(ref body, ref view, ref movementAI);

                if (!isGrounded) 
                    deathPool.Add(ent); //death
                else
                    standUpAnimationPool.Add(ent); //stand up event

                ragdollStatePool.Del(ent);
            }
        }
        

        private bool TryResetRagdoll(ref CharacterPhysicsBody body, ref CharacterView view, ref MovementAI ai)
        {
            foreach (var rb in body.BodyRagdoll)
            {
                rb.isKinematic = true;
            }

            view.Animator.enabled = true;
            body.Collider.enabled = true;
            
            var origin = view.HipBone.position;
            var isStandSuccess = ai.NavAgent.Warp(origin);

            return isStandSuccess;
        }
    }
}