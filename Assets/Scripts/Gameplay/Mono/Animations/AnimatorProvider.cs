using UnityEngine;
using System;

namespace BT
{
    public class AnimatorProvider : MonoBehaviour
    {
        public event Action<HitType> ActiveHitBoxEvent;

        private void ActiveHitBox(HitType type) => ActiveHitBoxEvent?.Invoke(type);
    }
}