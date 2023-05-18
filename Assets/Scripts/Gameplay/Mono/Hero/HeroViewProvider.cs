using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class HeroViewProvider : MonoBehaviour
    {        
        [field: SerializeField] public Transform CameraLookPoint {get; private set;}
        [field: SerializeField] public Transform CameraFollowPoint {get; private set;}
        [field: SerializeField] public Transform BodyHips {get; private set;}
        [field: SerializeField] public IEnumerable<HitBox> HitBoxes { get; private set; }
        [field: SerializeField] public IEnumerable<HurtBox> HurtBoxes { get; private set; }

        private bool _isInit;


        public void Init()
        {
            if (!_isInit)
            {
                HitBoxes = GetComponentsInChildren<HitBox>();
                HurtBoxes = GetComponentsInChildren<HurtBox>();
                
                foreach(var h in HurtBoxes) h.Init();

                _isInit = true;
            }
        }
    }
}