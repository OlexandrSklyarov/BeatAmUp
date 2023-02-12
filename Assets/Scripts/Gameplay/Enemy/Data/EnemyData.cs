using System;
using UnityEngine;

namespace BT
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "SO/EnemyData")]
    public class EnemyData : ScriptableObject    
    {
        [field: SerializeField] public EnemyConfig[] Enemies {get; private set;}        
    }


    [Serializable]
    public class EnemyConfig
    {
        [field: SerializeField] public EnemyType Type {get; private set;}        
        [field: SerializeField] public EnemyViewProvider Prefab {get; private set;} 
        [field: SerializeField, Min(1)] public int PoolSize {get; private set;} = 16;
    }
}