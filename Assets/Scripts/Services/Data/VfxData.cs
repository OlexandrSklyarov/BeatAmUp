using System;
using UnityEngine;

namespace BT
{
    [CreateAssetMenu(menuName = "SO/VfxConfig")]
    public class VfxData : ScriptableObject
    {
        [field: SerializeField] public VfxItem[] Items { get; private set; } 
    }


    [Serializable]
    public class VfxItem
    {
        [field: SerializeField] public VfxType Type { get; private set; }
        [field: SerializeField] public BaseVFXItem Prefab { get; private set; }
        [field: SerializeField, Min(4)] public int PoolSize { get; private set; } = 16;
    }
}