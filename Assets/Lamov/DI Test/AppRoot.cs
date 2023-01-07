﻿using Cysharp.Threading.Tasks;
using Lamov.DI.Runtime.Contexts;

namespace Lamov.DI_Test
{
    public class AppRoot : MonoBehaviourDiContext<AppRoot>
    {
        protected override async UniTask Bind()
        {
            await BindServices();
        }

        private async UniTask BindServices()
        {
            
        }
    }
}