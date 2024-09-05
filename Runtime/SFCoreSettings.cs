using System;

namespace SFramework.Core.Runtime
{
    [Serializable]
    public class SFCoreSettings : SFProjectSettings<SFCoreSettings>
    {
        public bool IsDebug;
    }
}