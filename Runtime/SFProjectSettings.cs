#pragma warning disable CS0162
using UnityEngine;

namespace SFramework.Core.Runtime
{
    public abstract class SFProjectSettings<T> : ScriptableObject where T : SFProjectSettings<T>
    {
        private static T _instance;

        public static bool TryGetInstance(out T instance)
        {
            _instance = Resources.Load<T>(typeof(T).Name);

            if (_instance == null)
            {
#if UNITY_EDITOR

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

                _instance = Resources.Load<T>(typeof(T).Name);

                if (_instance == null)
                {
                    instance = default;
                    return false;
                }
#else
                 _instance = CreateInstance<T>();
#endif

                instance = _instance;
                return true;
            }
            
            instance = _instance;
            return true;
        }
    }
}