using System.Threading.Tasks;
using UnityEngine;

namespace SFramework.Core.Runtime
{
    public abstract class SFView : MonoBehaviour, ISFInjectable
    {
        protected virtual void PreInit()
        {
        }
        
        protected virtual void Awake()
        {
            PreInit();
            SFContextRoot.Container.Inject(this);
        }

        
        [SFInject]
        protected virtual void Init()
        {
        }
    }
}