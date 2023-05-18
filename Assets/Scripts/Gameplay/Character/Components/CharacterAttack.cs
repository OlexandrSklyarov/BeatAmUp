using System.Collections.Generic;

namespace BT
{
    public struct CharacterAttack
    {
        public CharacterAttackAnimationData CurrentPunch;
        public CharacterAttackAnimationData CurrentKick;
        public Queue<CharacterAttackAnimationData> PunchQueue;
        public Queue<CharacterAttackAnimationData> KickQueue;        
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