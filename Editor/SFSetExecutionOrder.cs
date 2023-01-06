using SFramework.Core.Runtime;
using UnityEditor;
using UnityEngine;

namespace SFramework.Core.Editor
{
    [InitializeOnLoad]
    public class SFSetExecutionOrder
    {
        static SFSetExecutionOrder()
        {
            var scripts = (MonoScript[])Resources.FindObjectsOfTypeAll(typeof(MonoScript));
            foreach (var script in scripts)
            {
                if (!typeof(SFContextRoot).IsAssignableFrom(script.GetClass()) || script.GetClass().IsAbstract) continue;

                if (MonoImporter.GetExecutionOrder(script) >= 0)
                {
                    MonoImporter.SetExecutionOrder(script, -1);
                }
            }
        }
    }
}