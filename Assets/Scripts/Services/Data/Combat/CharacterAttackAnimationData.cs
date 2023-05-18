using System;
using UnityEngine;

namespace BT
{
    [Serializable]
    public sealed class CharacterAttackAnimationData
    {
        public float AttackTime => _clip.length / _animSpeed;

        [field: SerializeField] public string StateName {get; private set;}
        [field: SerializeField] public HitType HitType {get; private set;}
        [field: SerializeField, Range(0f, 1f)] public float DamageTimeMultiplier {get; private set;} = 0.65f;
        [SerializeField] private AnimationClip _clip;
        [SerializeField, Min(0.01f)] private float _animSpeed = 1f;
    }
}