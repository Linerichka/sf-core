using System;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SFramework.Core.Runtime
{
    public abstract class SFContextRoot : MonoBehaviour
    {
        protected SFContainer _container;

        protected virtual void Awake()
        {
            PreInit();
            _container = new SFContainer(gameObject);
            Bind(_container);
            _container.Inject();
        }

        private async UniTaskVoid Start()
        {
            await _container.InitServices(destroyCancellationToken);
            await Init(_container, destroyCancellationToken);
        }

        protected abstract void PreInit();
        protected abstract void Bind(SFContainer container);
        protected abstract UniTask Init(SFContainer container, CancellationToken cancellationToken);

        protected void OnDestroy()
        {
            _container.Dispose();
        }
    }
}