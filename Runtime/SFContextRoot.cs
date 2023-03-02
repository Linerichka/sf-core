using UnityEngine;

namespace SFramework.Core.Runtime
{
    public abstract class SFContextRoot : MonoBehaviour
    {
        public static ISFContainer Container => _container;

        private static SFContainer _container;
        
        protected virtual void Awake()
        {
            PreInit();
            _container = new SFContainer();
            Setup(_container);
            _container.Inject();
            Init(_container);
        }

        protected virtual void Start()
        {
            PostInit(_container);
        }

        protected abstract void PreInit();
        protected abstract void Setup(SFContainer container);
        protected abstract void Init(ISFContainer container);
        protected abstract void PostInit(ISFContainer container);
    }
}