using System.Threading.Tasks;
using UnityEngine;

namespace SFramework.Core.Runtime
{
    public abstract class SFView : MonoBehaviour, ISFInjectable
    {
        protected virtual void Awake()
        {
            PreInit();
            this.Inject();
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