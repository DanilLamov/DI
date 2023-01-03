using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Lamov.DI.Runtime.Contexts;

namespace Lamov.DI_Test
{
    public class AppRoot : RootDiContext<AppContext>
    {
        protected override async UniTask Bind()
        {
            await BindServices();
        }

        private async UniTask BindServices()
        {
            //_diContext.Container.Bind<ISceneLoader, SceneLoader>();
        }
    }
}