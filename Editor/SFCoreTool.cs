using System;
using SFramework.Core.Runtime;
using UnityEditor;
using UnityEngine;

namespace SFramework.Core.Editor
{
    [Serializable]
    public sealed class SFCoreTool : ISFEditorTool
    {
        [MenuItem("Edit/SFramework/Generate Database Scripts")]
        private static void GenerateScripts()
        {
            EditorUtility.DisplayProgressBar("Scripts Generation", "Wait...", 0);

            var guids = AssetDatabase.FindAssets($"t:{nameof(ScriptableObject)}");

            var databaseCodeGenerator = new SFDatabaseCodeGenerator();
            
            foreach (var guid in guids)
            {
                var database = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(ScriptableObject)) as ISFDatabaseGenerator;
                if(database == null) continue;
                database.GetGenerationData(out var generationData);
                databaseCodeGenerator.Generate(generationData);
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            EditorUtility.ClearProgressBar();
        }

        public string Title => "Core";
    }
}