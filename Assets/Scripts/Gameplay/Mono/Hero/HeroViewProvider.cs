using UnityEngine;

namespace Gameplay.Mono.Hero
{
    public class HeroViewProvider : MonoBehaviour
    {
        [field: SerializeField] public Transform CameraLookPoint {get; private set;}
        [field: SerializeField] public Transform CameraFollowPoint {get; private set;}       
    }
}