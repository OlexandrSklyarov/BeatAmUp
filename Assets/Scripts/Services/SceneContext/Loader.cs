using System.Collections.Generic;
using Services.Scenes.LoadingScreen;
using UnityEngine;

namespace Services.SceneContext
{
    public class Loader : MonoBehaviour
    {        
        private async void Start()
        {
            ProjectContext.Instance.Init();

            var operations = new Queue<ILoadingOperation>();
            operations.Enqueue(ProjectContext.Instance.AssetProvider);
            operations.Enqueue(ProjectContext.Instance.NextLevelSceneProvider);

            await ProjectContext.Instance.LoadingScreenProvider.LoadAndDestroy(operations);
        }
        
    }
}