using System;
using UnityEngine;

namespace SFramework.Core.Runtime
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SFTypeAttribute : PropertyAttribute
    {
        public SFTypeAttribute(Type databaseType, int targetLayer = -1)
        {
            DatabaseType = databaseType;
            TargetLayer = targetLayer;
        }

        public readonly Type DatabaseType;
        public readonly int TargetLayer;
    }
}