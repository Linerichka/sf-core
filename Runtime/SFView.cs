using System.Threading.Tasks;
using UnityEngine;

namespace SFramework.Core.Runtime
{
    public abstract class SFView : MonoBehaviour, ISFInjectable
    {
        [SFInject]
        private ISFContainer _container;

        protected virtual void Awake()
        {
            PreInit();
        }

        protected virtual void Start()
        {
            if (_container != null) return;
            SFContextRoot.Container.Inject(this);
        }

        protected virtual void PreInit()
        {
        }

        [SFInject]
        protected virtual void Init()
        {
        }
    }
}