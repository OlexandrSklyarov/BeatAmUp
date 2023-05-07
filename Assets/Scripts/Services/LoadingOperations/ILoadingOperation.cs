using System;
using System.Threading.Tasks;

namespace Services.Scenes.LoadingScreen
{
    public interface ILoadingOperation
    {
        Task Load(Action<float> onProgressCallback);
    }
}