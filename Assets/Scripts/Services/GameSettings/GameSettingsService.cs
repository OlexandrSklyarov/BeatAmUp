using UnityEngine;

namespace Services.GameSettings
{
    public class GameSettingsService
    {
        public int PlayerCount {get; private set;} = 1;

        public void SetPlayerCount(int count) => PlayerCount = Mathf.Min(1, count);
    }
}