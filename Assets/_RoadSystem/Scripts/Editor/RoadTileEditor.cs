using PathCreationEditor;
using UnityEditor;
using UnityEngine;

namespace RoadSystem
{
    [CustomEditor(typeof(RoadTile))]
    public class RoadTileEditor : PathEditor
    {
        [SerializeField] private bool _isFoldout = true;
        [SerializeField] private bool _isFoldoutSetting = true;

        public override void OnInspectorGUI()
        {
            _isFoldout = EditorGUILayout.Foldout(_isFoldout, "Neighbors");

            if (_isFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                var inProperty = serializedObject.FindProperty("_roadIns");
                var outProperty = serializedObject.FindProperty("_roadOuts");

                EditorGUILayout.PropertyField(inProperty);
                EditorGUILayout.PropertyField(outProperty);

                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel--;
            }

            _isFoldoutSetting = EditorGUILayout.Foldout(_isFoldoutSetting, "Settings");
            if (_isFoldoutSetting)
            {
                base.OnInspectorGUI();
            }
        }

        // protected override void OnSceneGUI()
        // {
        //     base.OnSceneGUI();
        // }
    }
}