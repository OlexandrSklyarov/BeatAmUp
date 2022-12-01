
using System.Collections;
using System.Collections.Generic;

namespace Gameplay.Character.Hero
{
    public struct HeroAttack
    {
        public PunchState CurrentPunchState;
        public int NextPunchState;
        public KickState CurrentKickState;
        public int NextKickState;
        public Queue<PunchState> PunchQueue;
        public Queue<KickState> KickQueue;
        public float AttackTimer;
        public bool IsActiveAttack;

        public float ResetNextActionTimer { get; internal set; }
    }
}