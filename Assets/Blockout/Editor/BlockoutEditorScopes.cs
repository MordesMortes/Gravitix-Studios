/* Radical Forge Copyright (c) 2017 All Rights Reserved
   </copyright>
   <author>Frederic Babord</author>
   <date>14th October 2017</date>
   <summary>Blockout scope extentions to auto dispose objects</summary>*/

using UnityEditor;
using UnityEngine;

namespace RadicalForge.Blockout
{
    public class UndoScope : GUI.Scope
    {
        private int m_currentGroup = 0;

        public UndoScope(string text)
        {
            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName(text);
            m_currentGroup = Undo.GetCurrentGroup();
        }

        protected override void CloseScope()
        {
            Undo.CollapseUndoOperations(m_currentGroup);
        }
    }

    public class HorizontalCenteredScope : GUI.Scope
    {
        public HorizontalCenteredScope(params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(options);
            GUILayout.FlexibleSpace();
        }

        public HorizontalCenteredScope(GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(style, options);
            GUILayout.FlexibleSpace();
        }

        public HorizontalCenteredScope(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(text, style, options);
            GUILayout.FlexibleSpace();
        }

        public HorizontalCenteredScope(Texture image, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(image, style, options);
            GUILayout.FlexibleSpace();
        }

        public HorizontalCenteredScope(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(content, style, options);
            GUILayout.FlexibleSpace();
        }

        protected override void CloseScope()
        {
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }

    public class VerticalCenteredScope : GUI.Scope
    {
        public VerticalCenteredScope(params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(options);
            GUILayout.FlexibleSpace();
        }

        public VerticalCenteredScope(GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(style, options);
            GUILayout.FlexibleSpace();
        }

        public VerticalCenteredScope(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(text, style, options);
            GUILayout.FlexibleSpace();
        }

        public VerticalCenteredScope(Texture image, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(image, style, options);
            GUILayout.FlexibleSpace();
        }

        public VerticalCenteredScope(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(content, style, options);
            GUILayout.FlexibleSpace();
        }

        protected override void CloseScope()
        {
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }
    }
}
