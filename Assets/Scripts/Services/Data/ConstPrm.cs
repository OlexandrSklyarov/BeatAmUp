
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
            public const string STAND_UP_FACE_DOWN = "STAND_UP_FACE_DOWN";
            public const string STAND_UP_FACE_UP = "STAND_UP_FACE_UP";
            public const string KNOCKED_OUT = "KNOCKED_OUT";            
        }


        public static class PrefsKey
        {
        }


        public static class Hero
        {
            public const float ACTION_TIME_MULTIPLIER = 2f;
            public const float VIEW_ENEMY_ANGLE = 45f;
            public const float ATTACK_RADIUS_MULTIPLIER = 6f;
            public const float TARGET_RADIUS_OFFSET = 0.5f;
            public const float MIN_VELOCITY_MULTIPLIER = 0.01f;
        }


        public static class Enemy
        {
            public const float VIEW_TARGET_RADIUS = 15f;
            public const float TARGET_ENCIRCLEMENT_RADIUS = 3f;
            public const float MIN_VELOCITY_OFFSET = 0.1f;
            public const float TARGET_FOCUS_DIST = 15f;
            public const float CHECK_SPAWN_TIME = 10f;
            public const int MAX_ENEMY_ON_LEVEL = 5;
        }


        public static class Character
        {
            public const float STUN_TIME = 3f;
            public const float POWER_STUN_TIME = 6f;
            public const float DEATH_TIME = 5f;
            public const float HIT_COUNT_RESET_TIME = 1f;
            public const int MAX_HIT_COUNT = 3;
            public const float RESTORE_RAGDOLL_TIME = 0.5f;
        }


        public static class UI
        {
            public const float CHANGE_HP_BAR_DURATION = 0.5f;
            public const int ENEMY_UI_ID = -1;
        }


        public static class AddressablesID
        {
            public const string LOADING_SCREEN = "LoadingScreen";
        }
    }
}