using UnityEngine;

namespace BT
{
    public class HeroViewProvider : MonoBehaviour
    {        
        [field: SerializeField] public Transform CameraLookPoint {get; private set;}
        [field: SerializeField] public Transform CameraFollowPoint {get; private set;}
        [field: SerializeField] public Transform BodyHips {get; private set;}
    }
}