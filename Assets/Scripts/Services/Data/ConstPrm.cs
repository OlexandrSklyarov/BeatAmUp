
namespace BT
{   
    public static class ConstPrm
    {
        public static class Animation
        {     
            public const string IDLE = "IDLE";
            public const string MOVE = "MOVE";
            public const string MOVE_SPEED = "SPEED";
            public const string DEATH = "DEATH";
            public const string ATTACK = "ATTACK";
            public const string ATTACK_SPEED_MULTIPLIER_PRM = "ATTACK_SPEED_MULTIPLIER";
            public const string RUN_SPEED_MULTIPLIER_PRM = "RUN_SPEED_MULTIPLIER";
            public const string FORWARD_PRM = "FORWARD";
            public const string SIDE_PRM = "SIDE";
            public const string GROUND = "GROUND";
            public const string SITTING = "SITTING";
            public const string JUMP = "JUMP";
            public const string FALLING = "FALLING";  
            public const string VERTICAL_VELOCITY = "VERTICAL_VELOCITY";
        }


        public static class PrefsKey
        {      
            public const string PLAYER_POINTS = "PlayerPoints";  
            public const string HERO_STATS = "HeroStats";       
        }  


        public static class Hero
        {                
            public const float ACTION_TIME_MULTIPLIER = 2f; 
        }
    }
}