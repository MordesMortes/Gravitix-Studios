using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RadicalForge.Blockout
{
    public class CommentExplorer : EditorWindow
    {
        private static class Styles
        {
            public static readonly string[] TabTypes = new string[]
            {
                "Area Comments",
                "Pin Comments",
                "All Comments"
            };
        }


        private Texture2D icon;

        [MenuItem("Window/Blockout/Comment Explorer", false, 12)]
        public static void Init()
        {
            var window = GetWindow<CommentExplorer>();
            window.minSize = new Vector2(500, 250);
            window.Show();
        }

        void OnEnable()
        {
            icon = Resources.Load(
                EditorGUIUtility.isProSkin
                    ? "Blockout/UI_Icons/Blockout_Icon_Light"
                    : "Blockout/UI_Icons/Blockout_Icon_Dark",
                typeof(Texture2D)) as Texture2D;

            titleContent = new GUIContent("Comment Explorer", icon);
            for (int i = 0; i < Styles.TabTypes.Length; ++i)
                windowTabs.Add(new CommentExplorerWindowTab().InitTab((CommentExplorerWindowTab.TabType) i));
        }

        private int selected = 0;
        private string searchString = "";
        private List<CommentExplorerWindowTab> windowTabs = new List<CommentExplorerWindowTab>();

        //private Notepad generalNotes;
        private Vector2 scrollView;

        void OnGUI()
        {
            windowTabs[selected].InitTab((CommentExplorerWindowTab.TabType) selected);

            //if (GameObject.Find("Blockout"))
            //    generalNotes = GameObject.Find("Blockout").GetComponent<Notepad>();

            using (var v = new EditorGUILayout.VerticalScope())
            {
                using (var h = new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
                {
                    selected = GUILayout.SelectionGrid(selected, Styles.TabTypes, Styles.TabTypes.Length,
                        EditorStyles.toolbarButton, GUILayout.Width(297));
                    GUILayout.FlexibleSpace();
                    searchString =
                        GUILayout.TextField(searchString, GUI.skin.FindStyle("ToolbarSeachTextField"),
                            GUILayout.MinWidth(150), GUILayout.MaxWidth(250));
                    if(GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                        GUI.Label(GUILayoutUtility.GetLastRect(), new GUIContent("", "Search in the scene for a matching comment name"));
                    GUILayout.Space(10);
                }

                GUILayout.Space(5);

                EditorGUI.indentLevel++;
                
                GUILayout.Space(5);


                using (var h = new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(EditorGUI.indentLevel * 15f);
                    windowTabs[selected].DrawTable(this, (CommentExplorerWindowTab.TabType) selected, searchString,
                        this.position.width);
                    GUILayout.Space(EditorGUI.indentLevel * 15f);
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}