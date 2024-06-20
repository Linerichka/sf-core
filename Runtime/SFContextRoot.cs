using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SFramework.Core.Runtime
{
    public abstract class SFContextRoot : MonoBehaviour
    {
        internal static SFContainer _Container => _container;
        protected static ISFContainer Container => _container;
        private static SFContainer _container;

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
        protected abstract UniTask Init(ISFContainer container, CancellationToken cancellationToken);
    }
}