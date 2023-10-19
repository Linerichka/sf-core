using System.Collections.Generic;

namespace SFramework.Core.Runtime
{
    public struct SFGenerationData
    {
        public SFGenerationData(string fileName, HashSet<string> properties)
        {
            FileName = fileName;
            Properties = properties;
        }

        public string FileName;
        public HashSet<string> Properties;
    }
}