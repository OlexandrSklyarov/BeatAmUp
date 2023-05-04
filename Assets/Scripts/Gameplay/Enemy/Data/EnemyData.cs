using System;
using UnityEngine;

namespace BT
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "SO/EnemyData")]
    public class EnemyData : ScriptableObject    
    {
        [field: SerializeField] public EnemyMovementConfig Movement {get; private set;}        
        [field: Space(20f), SerializeField] public EnemyAnimationConfig Animation {get; private set;}        
        [field: Space(20f), SerializeField] public EnemyPoolData[] EnemyPoolData {get; private set;}        
    }


    [Serializable]
    public class EnemyPoolData
    {
        [field: SerializeField] public EnemyType Type {get; private set;}        
        [field: SerializeField] public EnemyViewProvider Prefab {get; private set;} 
        [field: SerializeField, Min(1)] public int PoolSize {get; private set;} = 16;
    }


    [Serializable]
    public class EnemyMovementConfig
    {
        [field: SerializeField, Min(0.01f)] public float Speed {get; private set;} = 4f;      
        [field: SerializeField, Min(0.01f)] public float Acceleration {get; private set;} =8f;      
        [field: SerializeField, Min(0.01f)] public float AngularSpeed {get; private set;} = 800f;
    }


    [Serializable]
    public class EnemyAnimationConfig
    {   
        [field: SerializeField] public AnimationCurve ChangeSpeedCurve {get; private set;}  
        [field: SerializeField] public string StandUpFaceUpAnimationName {get; private set;}  
        [field: SerializeField] public string StandUpFaceDownAnimationName {get; private set;}  
    }
}