using SFramework.Core.Runtime;
using UnityEditor;
using UnityEngine;

namespace SFramework.Core.Editor
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class SFDefaultMonoBehaviourEditor : UnityEditor.Editor
    {
        private bool hideScriptField;

        private void OnEnable()
        {
            hideScriptField = target.GetType().GetCustomAttributes(typeof(SFHideScriptFieldAttribute), false).Length >
                              0;
        }

        public override void OnInspectorGUI()
        {
            if (hideScriptField)
            {
                serializedObject.Update();
                EditorGUI.BeginChangeCheck();
                DrawPropertiesExcluding(serializedObject, "m_Script");
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
            }
            else
            {
                base.OnInspectorGUI();
            }
        }
    }
}