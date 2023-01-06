using System;
using JetBrains.Annotations;
using UnityEngine.Scripting;

namespace SFramework.Core.Runtime
{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    public class SFInjectAttribute : PreserveAttribute
    {
    }
}