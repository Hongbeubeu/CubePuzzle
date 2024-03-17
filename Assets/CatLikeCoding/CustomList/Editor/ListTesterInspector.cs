using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ListTester)), CanEditMultipleObjects]
public class ListTesterInspector : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorList.Show(serializedObject.FindProperty("integers"), EditorListOption.All);
        EditorList.Show(serializedObject.FindProperty("vectors"), EditorListOption.All);
        EditorList.Show(serializedObject.FindProperty("colorPoints"), EditorListOption.All);
        EditorList.Show(serializedObject.FindProperty("objects"), EditorListOption.All);
        serializedObject.ApplyModifiedProperties();
    }
}