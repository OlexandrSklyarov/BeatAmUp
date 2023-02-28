using System;
using System.Collections.Generic;
using Services.Pooling;
using UnityEngine;
using BT;

namespace Gameplay.Factories
{
    public sealed class VfxFactory
    {
        private readonly Dictionary<VfxType, Pool<GameWorldVFX>> _poolCollection;


        public VfxFactory(VfxData config)
        {
            var root = new GameObject("[Factory - VFX]").transform;

            _poolCollection = new Dictionary<VfxType, Pool<GameWorldVFX>>();
            
            for (int i = 0; i < config.Items.Length; i++)
            {
                var item = config.Items[i];
                var containerName = $"[{item.Type}]";
                var pool = CreatePool(item, item.PoolSize, containerName, root);
                _poolCollection.Add(item.Type, pool);
            }
        }


        private Pool<GameWorldVFX> CreatePool(VfxItem vfxItem, int poolSize, string containerName, Transform root)
        {
            var container = new GameObject($"{containerName}").transform;
            container.SetParent(root);
            return new Pool<GameWorldVFX>(vfxItem.Prefab.gameObject, container, poolSize);
        }
      

        public BaseVFXItem GetVFX(VfxType vfxType)
        {
            var pool = GetPool(vfxType);
            return pool.GetElement();
        }
        
        
        public void ReturnToStorage(GameWorldVFX vfx)
        {
            var pool = GetPool(vfx.Type);
            pool.ReturnToPool(vfx);
        }


        private Pool<GameWorldVFX> GetPool(VfxType type)
        {            
            if (!_poolCollection.TryGetValue(type, out var pool))
            {
                throw new ArgumentException($"A VFX with this type {type} is not found.");
            }

            return pool;
        }
    }
}