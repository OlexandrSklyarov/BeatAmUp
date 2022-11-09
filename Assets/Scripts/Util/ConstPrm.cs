
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
        public const string JUMP = "JUMP";
        public const string FALLING = "FALLING";  
        public const string KICK_1 = "KICK_1";  
        public const string KICK_2 = "KICK_2";  
        public const string PUNCH_1 = "PUNCH_1"; 
        public const string PUNCH_2 = "PUNCH_2"; 
        public const string PUNCH_3 = "PUNCH_3"; 
        public const string PUNCH_4 = "PUNCH_4"; 
    }


    public static class PrefsKey
    {      
        public const string PLAYER_POINTS = "PlayerPoints";  
        public const string HERO_STATS = "HeroStats";       
    }  


    public static class Hero
    {      
        public const float DEFAULT_COMBO_TIMER = 0.4f; 
        public const float ATTACK_TIMER = 0.25f;               
    }
}