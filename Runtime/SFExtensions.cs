using UnityEngine;

namespace SFramework.Core.Runtime
{
    public static partial class SFExtensions
    {
        public static bool IsNone(this string value) => string.IsNullOrWhiteSpace(value);

        public static void Inject(this object behaviour)
        {
            if (SFContainer.Instance == null) return;
            
            SFContainer.Instance.Inject(behaviour);
        }
    }
}
