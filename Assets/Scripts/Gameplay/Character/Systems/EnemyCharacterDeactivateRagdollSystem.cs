using Leopotam.EcsLite;

namespace BT
{
    public sealed class EnemyCharacterDeactivateRagdollSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
                
            var enemies = world
                .Filter<RagdollState>()
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

            foreach (var ent in enemies)
            {
                ref var view = ref viewPool.Get(ent);
                ref var body = ref bodyPool.Get(ent);
                ref var movementAI = ref movementAiPool.Get(ent);
                 
                ResetRagdoll(ref body, ref view, ref movementAI);

                ragdollStatePool.Del(ent);
            }
        }
        

        private void ResetRagdoll(ref CharacterPhysicsBody body, ref CharacterView view, ref MovementAI ai)
        {
            foreach (var rb in body.BodyRagdoll)
            {
                rb.isKinematic = true;
            }

            view.Animator.enabled = true;
            body.Collider.enabled = true;
            
            var origin = view.HipBone.position;
            ai.NavAgent.Warp(origin);
        }
    }
}