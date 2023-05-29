using System;
using System.Threading.Tasks;
using Services.Scenes.LoadingScreen;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Services.AddressablesProvider
{
    public class AssetProvider : ILoadingOperation
    {
        public string Description => "Assets Initialization...";

        private bool _isReady;


        public async Task Load(Action<float> onProgressCallback)
        {
            var op = Addressables.InitializeAsync();
            await op.Task;
            _isReady = true;
        }


        public async Task<SceneInstance> LoadAdditiveScene(string sceneId)
        {
            await WaitUntilReady();
            var op = Addressables.LoadSceneAsync(sceneId, LoadSceneMode.Additive);
            return await op.Task;
        }


        public async Task<SceneInstance> LoadAdditiveScene(Scene scene)
        {
            await WaitUntilReady();
            var op = Addressables.LoadSceneAsync(scene, LoadSceneMode.Additive);
            return await op.Task;
        }


        public async Task UploadAdditiveScene(SceneInstance scene)
        {
            await WaitUntilReady();
            var op = Addressables.UnloadSceneAsync(scene);
            await op.Task;
        }


        private async Task WaitUntilReady()
        {
            while(!_isReady) await Task.Yield();
        }
    }
}