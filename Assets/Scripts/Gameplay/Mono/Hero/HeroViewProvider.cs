using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class HeroViewProvider : MonoBehaviour, IHeroViewProvider
    {        
        [field: SerializeField] public Transform Transform {get; private set;}
        [field: SerializeField] public Transform CameraLookPoint {get; private set;}
        [field: SerializeField] public Transform CameraFollowPoint {get; private set;}
        [field: SerializeField] public Transform BodyHips {get; private set;}
        [field: SerializeField] public IEnumerable<HitBox> HitBoxes { get; private set; }
        [field: SerializeField] public IEnumerable<HurtBox> HurtBoxes { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public CharacterController CC  { get; private set; }

        private bool _isInit;


        public void Init()
        {
            if (!_isInit)
            {
                Transform = transform;
                HitBoxes = GetComponentsInChildren<HitBox>();
                HurtBoxes = GetComponentsInChildren<HurtBox>();
                Animator = GetComponentInChildren<Animator>();
                CC = GetComponent<CharacterController>();

                foreach(var h in HurtBoxes) 
                {
                    h.Init();
                }

                _isInit = true;
            }
        }
    }
}