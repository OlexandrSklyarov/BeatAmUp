using System.Collections.Generic;
using Services.Scenes.LoadingScreen;
using UnityEngine;

namespace Services.Scenes
{
    public class Loader : MonoBehaviour
    {        
        private async void Start()
        {
            ProjectContext.Instance.Init();

            var operations = GetLoadingOperations();

            await ProjectContext.Instance.LoadingScreenProvider.LoadAndDestroy(operations);
        }


        private static Queue<ILoadingOperation> GetLoadingOperations()
        {
            var operations = new Queue<ILoadingOperation>();
            
            operations.Enqueue(ProjectContext.Instance.AssetProvider);
            operations.Enqueue(ProjectContext.Instance.NextLevelSceneProvider);

            return operations;
        }
    }
}