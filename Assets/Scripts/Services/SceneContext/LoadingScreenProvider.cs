using BT;
using System.Threading.Tasks;
using Services.Scenes.LoadingScreen;
using System.Collections.Generic;
using Services.AddressablesProvider;

namespace Services.SceneContext
{
    public class LoadingScreenProvider : BaseLocalAssetProvider
    {
        public async Task LoadAndDestroy(Queue<ILoadingOperation> operations)
        {
            var loadingScreen = await Load();
            await loadingScreen.LoadAsync(operations);
            Upload();
        }


        public Task<LoadingScreenView> Load()
        {
            return LoadInternal<LoadingScreenView>(ConstPrm.AddressablesID.LOADING_SCREEN);
        }


        public void Upload() => UploadInternal();
    }
}