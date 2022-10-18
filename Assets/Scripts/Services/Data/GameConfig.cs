using System;
using UnityEngine;

namespace Services.Data
{
    [CreateAssetMenu(menuName = "SO/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [field: SerializeField] public PlayerConfig PlayerData { get; private set; }
    }


    [Serializable]
    public sealed class PlayerConfig
    {
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField, Min(1f)] public float Speed { get; private set; } = 2f;
    }
}