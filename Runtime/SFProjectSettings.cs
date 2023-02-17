#pragma warning disable CS0162
using System.IO;
using UnityEngine;

namespace SFramework.Core.Runtime
{
    public abstract class SFProjectSettings<T> : ScriptableObject where T : SFProjectSettings<T>
    {
        private const string _folderPath = "SFramework/Settings/";
        public static readonly string _assetPath = $"Assets/{_folderPath}{typeof(T).Name}.asset";

        public static bool Instance(out T result)
        {
            result = null;
            
#if UNITY_EDITOR
            var path = Path
                .GetFullPath(Path.Combine(Application.dataPath + Path.DirectorySeparatorChar + _folderPath));

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                UnityEditor.AssetDatabase.Refresh();
            }

            var settings = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(_assetPath);
            if (settings == null)
            {
                settings = CreateInstance<T>();
                UnityEditor.AssetDatabase.CreateAsset(settings, _assetPath);
                UnityEditor.AssetDatabase.SaveAssetIfDirty(settings);
                UnityEditor.AssetDatabase.Refresh();
            }
            
            result = settings;
            return true;
#endif
            return false;
        }
    }
}