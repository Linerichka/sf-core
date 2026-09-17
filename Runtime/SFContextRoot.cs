using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SFramework.Core.Runtime
{
    public abstract class SFContextRoot : MonoBehaviour
    {
        protected SFContainer _container;
        
        public async UniTask Init()
        {
            gameObject.SetActive(false);
            
            PreInit();
            
            _container = new SFContainer(gameObject);
            Bind(_container);
            _container.Inject();
            
            await _container.InitServices(destroyCancellationToken);
            await PostInit(_container, destroyCancellationToken);
            
            gameObject.SetActive(true);
        }

        protected abstract void PreInit();
        protected abstract void Bind(SFContainer container);
        protected abstract UniTask PostInit(SFContainer container, CancellationToken cancellationToken);

        
        protected virtual void OnDestroy()
        {
            _container.Dispose();
        }
    }
}