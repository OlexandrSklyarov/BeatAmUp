using System.Collections.Generic;
using System.Threading.Tasks;
using Services.Scenes.LoadingScreen;
using UnityEngine;

namespace Services.Scenes
{
    public class Loader : MonoBehaviour
    {        
        private async void Start()
        {
            ProjectContext.Instance.Init();

            var operations = new Queue<ILoadingOperation>();
                        
            //Choose menu
            operations.Enqueue(ProjectContext.Instance.AssetProvider);
            operations.Enqueue(ProjectContext.Instance.ChoosePlayerOperation);
            await ProjectContext.Instance.LoadingScreenProvider.LoadAndDestroy(operations);

            await ProjectContext.Instance.ChoosePlayerOperation.WaitChoose();

            operations.Clear();

            //Game
            operations.Enqueue(ProjectContext.Instance.NextLevelSceneProvider);
            await ProjectContext.Instance.LoadingScreenProvider.LoadAndDestroy(operations);
        }        

        private async Task WaitLoadOperations()
        {
            while(LoadingScreenView.Instance != null) await Task.Delay(100);
        }
    }
}