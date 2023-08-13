using Services.AddressablesProvider;
using UnityEngine;
using Services.Scenes;

namespace Services
{
    public class ProjectContext : MonoBehaviour
    {        
        public LoadingScreenProvider LoadingScreenProvider {get; private set;}
        public AssetProvider AssetProvider{ get; private set; }
        public NextLevelSceneProvider NextLevelSceneProvider { get; private set; }

        public static ProjectContext Instance;

        private void Awake()
        {
            Instance = this;

            DontDestroyOnLoad(this);
        }


        public void Init()
        {
            LoadingScreenProvider = new LoadingScreenProvider();
            AssetProvider = new AssetProvider();
            NextLevelSceneProvider = new NextLevelSceneProvider();
        }        
    }
}