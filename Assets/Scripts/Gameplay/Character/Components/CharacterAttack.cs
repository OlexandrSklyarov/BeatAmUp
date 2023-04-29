using System.Collections.Generic;

namespace BT
{
    public struct CharacterAttack
    {
        public HeroAttackAnimationData CurrentPunch;
        public HeroAttackAnimationData CurrentKick;
        public Queue<HeroAttackAnimationData> PunchQueue;
        public Queue<HeroAttackAnimationData> KickQueue;        
        public int NextPunchState;
        public int NextKickState;
        public float AttackTimer;
        public float ResetNextActionTimer;
        public bool IsActiveAttack;
        public bool IsNeedFinishAttack;
        public bool IsPowerfulDamage;
        public int HitCount;
        public float HitResetTimer;
    }
}