using Gameplay.Mono.Character;
using UnityEngine;

namespace Gameplay.Mono.Hero
{
    public class HeroViewProvider : MonoBehaviour
    {
        [field: SerializeField] public Transform CameraLookPoint {get; private set;}
        [field: SerializeField] public Transform CameraFollowPoint {get; private set;}       
        [field: SerializeField] public CharacterCollisionView CollisionView {get; private set;}       
    }
}