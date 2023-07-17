using UnityEngine;
using System.Collections.Generic;

namespace BT
{
    public interface IHeroViewProvider
    {
        Transform Transform { get;}
        Transform CameraLookPoint {get;}
        Transform CameraFollowPoint {get;}
        Transform BodyHips {get; }
        IEnumerable<HitBox> HitBoxes { get; }
        IEnumerable<HurtBox> HurtBoxes { get;}
        Animator Animator { get;}
        CharacterController CC { get;}

        void Init();
    }
}