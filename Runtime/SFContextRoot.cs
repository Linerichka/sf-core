using UnityEngine;

namespace SFramework.Core.Runtime
{
    public abstract class SFContextRoot : MonoBehaviour
    {
        public static ISFContainer Container => _container;

        private static SFContainer _container;
        
        protected virtual void Awake()
        {
            _container = new SFContainer();
            PreInit();
            Setup(_container);
            _container.Inject();
        }

        protected virtual void Start()
        {
            Init(_container);
        }

        protected abstract void PreInit();
        protected abstract void Setup(SFContainer container);
        protected abstract void Init(ISFContainer container);
    }
}