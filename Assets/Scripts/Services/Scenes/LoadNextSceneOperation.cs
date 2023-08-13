using System;
using System.Threading.Tasks;
using Services.Scenes.LoadingScreen;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Services.Scenes
{
    public class LoadNextSceneOperation : ILoadingOperation
    {
        private Action<float> _progressCallback;
        private readonly string _nextSceneName;
        

        public LoadNextSceneOperation(string nextSceneName)
        {
            _nextSceneName = nextSceneName;
        }


        public async Task Load(Action<float> onProgressCallback)
        {
            _progressCallback = onProgressCallback;
            _progressCallback?.Invoke(0.3f);
            await LoadNextSceneAsync(_nextSceneName);
            
            await Task.Delay(TimeSpan.FromSeconds(0.15f));
            
            _progressCallback?.Invoke(1f);
        }
        

        private async Task LoadNextSceneAsync(string sceneName)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.15f));
            
            _progressCallback?.Invoke(0.6f);

            var operation = SceneManager.LoadSceneAsync(sceneName);

            while (!operation.isDone)
            {
                await Task.Delay(1);
            }

            _progressCallback?.Invoke(0.9f);
            
            Debug.Log($"Load scene -{sceneName}- completed!!!");
        }
    }
}