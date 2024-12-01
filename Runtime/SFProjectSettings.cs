#pragma warning disable CS0162
using UnityEngine;

namespace SFramework.Core.Runtime
{
    public abstract class SFProjectSettings<T> : ScriptableObject where T : SFProjectSettings<T>
    {
        private static T _instance;

        public static bool Instance(out T result)
        {
            result = null;

#if UNITY_EDITOR
            if (_instance == null)
            {
                var settings = Resources.Load<T>(typeof(T).Name);
                if (settings == null)
                {
                  
                    
                    if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/SFramework"))
                    {
                        UnityEditor.AssetDatabase.CreateFolder("Assets", "SFramework");
                    }
                    
                    if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/SFramework/Resources"))
                    {
                        UnityEditor.AssetDatabase.CreateFolder("Assets/SFramework", "Resources");
                    }
                    
                    UnityEditor.AssetDatabase.CreateAsset(CreateInstance<T>(), "Assets/SFramework/Resources/" + typeof(T).Name + ".asset");
                    UnityEditor.AssetDatabase.ImportAsset("Assets/SFramework/Resources/" + typeof(T).Name + ".asset", UnityEditor.ImportAssetOptions.ForceUpdate);
                    UnityEditor.AssetDatabase.Refresh();
                    settings = Resources.Load<T>(typeof(T).Name);
                }

                result = settings;
                _instance = result;
            }
            else
            {
                result = _instance;
            }
            
            return true;
#endif
            return false;
        }
    }
}