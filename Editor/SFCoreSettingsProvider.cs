using System.Collections.Generic;
using SFramework.Core.Runtime;
using UnityEditor;

namespace SFramework.Core.Editor
{
    public static class SFCoreSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider Create()
        {
            var provider = new SettingsProvider("Project/SFramework/Core", SettingsScope.Project)
            {
                guiHandler = (searchContext) =>
                {
                    if (!SFCoreSettings.Instance(out var settings)) return;
                    var settingsSO = new SerializedObject(settings);
                    EditorGUILayout.PropertyField(settingsSO.FindProperty("generatorScriptsPath"));
                    EditorGUILayout.PropertyField(settingsSO.FindProperty("isDebug"));
                    settingsSO.ApplyModifiedPropertiesWithoutUndo();
                    AssetDatabase.SaveAssetIfDirty(settingsSO.targetObject);
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "SF", "Core" })
            };

            return provider;
        }
    }
}