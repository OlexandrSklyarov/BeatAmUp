using System.Threading.Tasks;
using System.Collections.Generic;
using Services.AddressablesProvider;
using BT;

namespace Services.Scenes.LoadingScreen
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