using UnityEngine;

namespace SFramework.Core.Runtime
{
    public static partial class SFExtensions
    {
        public static bool IsNone(this string value) => string.IsNullOrWhiteSpace(value);

        public static void Inject(this MonoBehaviour behaviour)
        {
            SFContextRoot.Container.Inject(behaviour);
        }
    }
}