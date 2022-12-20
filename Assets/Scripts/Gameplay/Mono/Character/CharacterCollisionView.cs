using UnityEngine;
using Leopotam.EcsLite;

namespace BT
{
    public class CharacterCollisionView : MonoBehaviour
    {
        private EcsPackedEntity _packedEntity;
        private EcsWorld _world;
        private LayerMask _groundLayer;


        public void Init(EcsWorld world, int entity, LayerMask groundLayer)
        {
            _world = world;
            _packedEntity = _world.PackEntity(entity);
            _groundLayer = groundLayer;
        } 


        private void OnCollisionEnter(Collision other) 
        {
            if (IsGroundLayer(other.gameObject.layer) && IsEntityAlive(out int entity))
            {   
                var pool = _world.GetPool<CharacterGrounded>();  
                if (!pool.Has(entity)) pool.Add(entity);
            }
        }


        private void OnCollisionExit(Collision other) 
        {
            if (IsGroundLayer(other.gameObject.layer) && IsEntityAlive(out int entity))
            {
                var pool = _world.GetPool<CharacterGrounded>();  
                if (pool.Has(entity)) pool.Del(entity);
            }
        }


        private bool IsEntityAlive(out int entity)
        {
            return _packedEntity.Unpack(_world, out entity);
        }


        private bool IsGroundLayer(LayerMask other)
        {
            return other.value == _groundLayer.value;
        }
    }
}