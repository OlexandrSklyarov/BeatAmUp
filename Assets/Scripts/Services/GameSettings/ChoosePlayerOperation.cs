using System.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;
using System.Linq;
using BT;
using Services.Scenes.LoadingScreen;
using System;

namespace Services.GameSettings
{
    public class ChoosePlayerOperation : ILoadingOperation
    {
        private string sceneName => "ChoosePlayer";

        private PlayerChooseScreen _chooseScreen;
        private SceneInstance _currentScene;
        private bool _isChooseCompleted;

        public async Task Load(Action<float> onProgressCallback)
        {
            onProgressCallback?.Invoke(0.2f);

            _currentScene = await ProjectContext.Instance.AssetProvider.LoadAdditiveScene(sceneName);   

            _chooseScreen = _currentScene.Scene
                .GetRootGameObjects()
                .Select(x => x.GetComponentInChildren<PlayerChooseScreen>())
                .First();                 

            await Task.Delay(TimeSpan.FromSeconds(0.25f));   

            onProgressCallback?.Invoke(1f);     
        }

        public async Task WaitChoose()
        {
            if (_chooseScreen == null) return;

            _isChooseCompleted = false;            
            _chooseScreen.ChooseCompletedEvent += CompletedHandler;

            while(!_isChooseCompleted) await Task.Yield();
            
            await ProjectContext.Instance.AssetProvider.UploadAdditiveScene(_currentScene);           
        }


        private void CompletedHandler()
        {
            _chooseScreen.ChooseCompletedEvent -= CompletedHandler;
            _isChooseCompleted = true;
        }        
    }
}