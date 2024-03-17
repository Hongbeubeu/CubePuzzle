using UnityEditor;
using UnityEngine;

public static class EditorList
{
    public static void Show(SerializedProperty list, EditorListOption options = EditorListOption.Default)
    {
        if (!list.isArray)
        {
            EditorGUILayout.HelpBox($" {list.name} is neither an array nor a list!", MessageType.Error);
            return;
        }

        var showListLabel = (options & EditorListOption.ListLabel) != 0;
        var showListSize = (options & EditorListOption.ListSize) != 0;

        var size = list.FindPropertyRelative("Array.size");
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        if (showListLabel)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.indentLevel += 1;
            list.isExpanded = EditorGUILayout.Foldout(list.isExpanded, list.displayName);
            if (showListSize)
            {
                EditorGUILayout.LabelField(new GUIContent("length"), GUILayout.MaxWidth(60));
                EditorGUILayout.PropertyField(size, GUIContent.none, GUILayout.MaxWidth(40));
            }

            EditorGUILayout.EndHorizontal();
        }

        if (!showListLabel || list.isExpanded)
        {
            if (size.hasMultipleDifferentValues)
            {
                EditorGUILayout.HelpBox("Not showing lists with different sizes.", MessageType.Info);
            }
            else
            {
                ShowElements(list, options);
            }
        }

        if (showListLabel)
            EditorGUI.indentLevel -= 1;
        EditorGUILayout.EndVertical();
    }

    private static GUIContent
        moveButtonContent = new GUIContent("\u21b4", "move down"),
        duplicateButtonContent = new GUIContent("+", "duplicate"),
        deleteButtonContent = new GUIContent("-", "delete"),
        addButtonContent = new GUIContent("+", "add element");

    private static void ShowElements(SerializedProperty list, EditorListOption options)
    {
        var showElementLabels = (options & EditorListOption.ElementLabels) != 0;
        var showButtons = (options & EditorListOption.Buttons) != 0;


        for (var i = 0; i < list.arraySize; i++)
        {
            if (showButtons)
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
            }

            if (showElementLabels)
            {
                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
            }
            else
            {
                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), GUIContent.none);
            }

            if (showButtons)
            {
                ShowButtons(list, i);
                EditorGUILayout.EndHorizontal();
            }
        }
        
        if (showButtons && list.arraySize == 0 && GUILayout.Button(addButtonContent, EditorStyles.miniButton))
        {
            list.arraySize += 1;
        }
    }

    private static readonly GUILayoutOption miniButtonWidth = GUILayout.Width(20f);

    private static void ShowButtons(SerializedProperty list, int index)
    {
        if (GUILayout.Button(moveButtonContent, EditorStyles.miniButtonLeft, miniButtonWidth))
        {
            list.MoveArrayElement(index, index + 1);
        }

        if (GUILayout.Button(duplicateButtonContent, EditorStyles.miniButtonMid, miniButtonWidth))
        {
            list.InsertArrayElementAtIndex(index);
        }

        if (GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonRight, miniButtonWidth))
        {
            var oldSize = list.arraySize;
            list.DeleteArrayElementAtIndex(index);
            if (list.arraySize == oldSize)
            {
                list.DeleteArrayElementAtIndex(index);
            }
        }
    }
}