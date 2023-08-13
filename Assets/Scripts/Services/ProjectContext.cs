using UnityEngine;
using Services.Scenes.LoadingScreen;
using Services.AddressablesProvider;
using Services.Scenes;
using Services.Input;
using Services.GameSettings;
using BT;

namespace Services
{
    public class ProjectContext : MonoBehaviour
    {        
        public LoadingScreenProvider LoadingScreenProvider {get; private set;}
        public ChoosePlayerOperation ChoosePlayerOperation { get; internal set; }
        public AssetProvider AssetProvider{ get; private set; }
        public NextLevelSceneProvider NextLevelSceneProvider { get; private set; }
        public PlayerBindInputService PlayerBindInputService { get; private set; }
        public GameSettingsService GameSettings {get; private set;}
        public GameConfig Config => _gameConfig;


        [SerializeField] private GameConfig _gameConfig;

        public static ProjectContext Instance;

        private void Awake()
        {
            Instance = this;

            DontDestroyOnLoad(this);
        }


        public void Init()
        {
            LoadingScreenProvider = new LoadingScreenProvider();
            ChoosePlayerOperation = new ChoosePlayerOperation();
            AssetProvider = new AssetProvider();
            NextLevelSceneProvider = new NextLevelSceneProvider();
            PlayerBindInputService = new PlayerBindInputService(_gameConfig.GameRules);
            GameSettings = new GameSettingsService();
        }        
    }
}