using UnityEngine;

namespace SFramework.Core.Runtime
{
    public class SFCoreSettings : SFProjectSettings<SFCoreSettings>
    {
        public string GeneratorScriptsPath => generatorScriptsPath;
        public bool IsDebug => isDebug;
        
        [SerializeField]
        private string generatorScriptsPath = "SFramework/Generated";

        [SerializeField]
        private bool isDebug;
    }
}