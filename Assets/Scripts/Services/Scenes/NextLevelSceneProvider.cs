using System;
using System.Threading.Tasks;
using Services.Scenes.LoadingScreen;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Services.Scenes
{
    public class NextLevelSceneProvider : ILoadingOperation
    {
        private string NextScene => $"Level{_nextIndex}";

        private SceneInstance _currentLevel;
        private int _nextIndex = 1; 
        private bool _isLevelLoaded;

        private const int MAX_LEVEL_COUNT = 1; 


        public async Task Load(Action<float> onProgressCallback)
        {
            onProgressCallback?.Invoke(0.2f);

            if (_isLevelLoaded)
            {
                await ProjectContext.Instance.AssetProvider.UploadAdditiveScene(_currentLevel);
                
                _nextIndex++;
                _nextIndex %= MAX_LEVEL_COUNT;
            }

            onProgressCallback?.Invoke(0.5f);

            _currentLevel = await ProjectContext.Instance.AssetProvider.LoadAdditiveScene(NextScene);
            _isLevelLoaded = true;

            onProgressCallback?.Invoke(0.95f);

            await Task.Delay(TimeSpan.FromSeconds(1f));

            onProgressCallback?.Invoke(1f);
        }
    }
}