using System;
using UnityEngine;

namespace BT
{
    [Serializable]
    public class HeroAttackAnimationData
    {
        [field: SerializeField] public string StateName {get; private set;}
        [field: SerializeField] public HitType HitType {get; private set;}
        public float AttackTime => _clip.length / _animSpeed;
        [SerializeField] private AnimationClip _clip;
        [SerializeField, Min(0.01f)] private float _animSpeed = 1f;
    }
}