
namespace BT
{
    public static class ConstPrm
    {
        public static class DevicesName
        {
            public const string KEYBOARD = "Keyboard";
            public const string GAMEPAD = "Gamepad";
        }

        public static class Animation
        {
            public const string IDLE = "IDLE";
            public const string MOVE = "MOVE";
            public const string MOVE_SPEED = "SPEED";
            public const string DEATH = "DEATH";
            public const string DAMAGE = "DAMAGE";
            public const string STUN = "STUN";
            public const string THROW_BODY = "THROW_BODY";
            public const string DAMAGE_TYPE = "DAMAGE_TYPE";
            public const string HAMMERING_DAMAGE = "HAMMERING_DAMAGE";
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
        }


        public static class Hero
        {
            public const float ACTION_TIME_MULTIPLIER = 2f;
            public const float VIEW_ENEMY_ANGLE = 85f;
            public const float SLIDE_TO_TARGET_TIME = 0.15f;
            public const float ATTACK_RADIUS_MULTIPLIER = 2f;
            public const float MIN_VELOCITY_MULTIPLIER = 0.01f;
        }


        public static class Enemy
        {
            public const float VIEW_TARGET_RADIUS = 15f;
            public const float TARGET_ENCIRCLEMENT_RADIUS = 3f;
            public const float MIN_VELOCITY_OFFSET = 0.1f;
        }


        public static class Character
        {
            public const float STUN_TIME = 3f;
            public const float POWER_STUN_TIME = 6f;
            public const float DEATH_TIME = 5f;
            public const float HIT_COUNT_RESET_TIME = 1f;
            public const int MAX_HIT_COUNT = 3;
            public const string HIT_LAYER_NAME = "HIT";
        }


        public static class UI
        {
            public const float CHANGE_HP_BAR_DURATION = 0.5f;
        }
    }
}