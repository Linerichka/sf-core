using System;
using System.Collections.Generic;
using System.Linq;
using SFramework.Core.Runtime;
using UnityEditor;
using UnityEngine;

namespace SFramework.Core.Editor
{
    [CustomPropertyDrawer(typeof(SFTypeAttribute), true)]
    public class SFTypeAttributeDrawer : PropertyDrawer
    {
        private ISFDatabase _database;

        private bool CheckAndLoadDatabase(ref Rect position, ref SerializedProperty property, ref GUIContent label,
            Type databaseType)
        {
            if (_database != null) return true;

            var typeName = databaseType.Name;

            var assetsGuids = AssetDatabase.FindAssets($"t:{typeName}");

            if (assetsGuids == null || assetsGuids.Length == 0)
            {
                Debug.LogWarning($"Missing Database: {typeName}");
                return false;
            }

            var path = AssetDatabase.GUIDToAssetPath(assetsGuids.First());
            _database = AssetDatabase.LoadAssetAtPath(path, databaseType) as ISFDatabase;

            return _database != null;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                GUI.backgroundColor = Color.red;
                EditorGUI.LabelField(position, "Use string field!");
                GUI.backgroundColor = Color.white;
                return;
            }

            var sfTypeAttribute = attribute as SFTypeAttribute;

            if (!CheckAndLoadDatabase(ref position, ref property, ref label, sfTypeAttribute.DatabaseType)) return;

            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;


            if (string.IsNullOrWhiteSpace(property.stringValue))
            {
                property.stringValue = string.Empty;
            }

            var paths = new List<string> { "-" };
            _database.Nodes.FindAllPaths(out var ids, sfTypeAttribute.TargetLayer);
            paths.AddRange(ids);

            if (!string.IsNullOrWhiteSpace(property.stringValue) && !paths.Contains(property.stringValue))
            {
                GUI.backgroundColor = Color.red;
                property.stringValue = EditorGUI.TextField(position, property.stringValue);
                GUI.backgroundColor = Color.white;
            }
            else
            {
                var name = paths.Contains(property.stringValue)
                    ? property.stringValue
                    : paths[0];

                var _index = paths.IndexOf(name);

                EditorGUI.BeginChangeCheck();

                if (_index == 0)
                {
                    GUI.backgroundColor = Color.red;
                }

                _index = EditorGUI.Popup(position, _index, paths.ToArray());

                GUI.backgroundColor = Color.white;

                if (EditorGUI.EndChangeCheck())
                {
                    property.stringValue = _index == 0 ? string.Empty : paths.ElementAt(_index);
                }
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}