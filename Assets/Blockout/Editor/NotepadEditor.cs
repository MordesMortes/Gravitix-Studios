/* Radical Forge Copyright (c) 2017 All Rights Reserved
   </copyright>
   <author>Frederic Babord</author>
   <date>05th July 2017</date>
   <summary>
        How the notepad should be displayed in the inspector window
   </summary>*/

using UnityEditor;
using UnityEngine;

namespace RadicalForge.Blockout
{

    [CustomEditor(typeof(Notepad)), CanEditMultipleObjects]
    public class NotepadEditor : Editor
    {


        public SerializedProperty general, todo, other;

        void OnEnable()
        {
            general = serializedObject.FindProperty("generalNotes");
            todo = serializedObject.FindProperty("toDoNotes");
            other = serializedObject.FindProperty("otherNotes");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.LabelField("General Notes", EditorStyles.boldLabel);
            general.stringValue = EditorGUILayout.TextArea(general.stringValue, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 2));
            EditorGUILayout.LabelField("TODO Notes", EditorStyles.boldLabel);
            todo.stringValue = EditorGUILayout.TextArea(todo.stringValue, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 2));
            EditorGUILayout.LabelField("Other Notes", EditorStyles.boldLabel);
            other.stringValue = EditorGUILayout.TextArea(other.stringValue, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 2));
            serializedObject.ApplyModifiedProperties();
        }
    }

}