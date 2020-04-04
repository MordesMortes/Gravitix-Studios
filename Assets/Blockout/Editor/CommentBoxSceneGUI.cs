/* Radical Forge Copyright (c) 2017 All Rights Reserved
   </copyright>
   <author>Frederic Babord</author>
   <date>15th June 2017</date>
   <summary>Draws the name of the comment box in the scene view</summary>*/

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace RadicalForge.Blockout
{

    [ExecuteInEditMode]
    public class CommentBoxSceneGUI : EditorWindow
    {

        private static string targetName = "";
        private static string generalNoteData = "";
        private static string toDoNoteData = "";
        private static string otherNoteData = "";
        private static GUIStyle style;
        private static Vector3 camPos = Vector3.zero;
        public static bool ShowCommentInfo = true;
        public static bool ShowCommentInfoInternal = false;
        public static bool showPinComments = true, showAreaComments = true;
        public static bool showPinCommentsInternal = true, showAreaCommentsInternal = true;

        public static Notepad GlobalNotes;
        static GUIStyle labelStyle = new GUIStyle();

        public static void Enable()
        {
            SceneView.onSceneGUIDelegate += OnScene;
			var giz = FindObjectsOfType<BlockoutSceneViewCubeGizmo> ().ToList ();
			giz.ForEach(x => {x.commentsActive = true; x.Update();});
            var pins = FindObjectsOfType<BlockoutPinGizmo>().ToList();
			pins.ForEach(x => {x.commentsActive = true; x.Update();});

            if (SceneView.lastActiveSceneView)
                SceneView.lastActiveSceneView.Repaint();
            ShowCommentInfo = true;

        }

        public static void Disable()
        {
            var giz = FindObjectsOfType<BlockoutSceneViewCubeGizmo>().ToList();
            giz.ForEach(x => {
                x.commentsActive = false;
                x.Update();
            });
            var pins = FindObjectsOfType<BlockoutPinGizmo>().ToList();
            pins.ForEach(x => {
                x.commentsActive = false;
                x.Update();
            });

            if (Selection.activeGameObject)
            {
                if (Selection.activeGameObject.GetComponent<BlockoutSceneViewCubeGizmo>() ||
                    Selection.activeGameObject.GetComponent<BlockoutPinGizmo>())
                    Selection.activeGameObject = null;
            }

            if (SceneView.onSceneGUIDelegate != null)
                SceneView.onSceneGUIDelegate -= OnScene;
            if(SceneView.lastActiveSceneView)
                SceneView.lastActiveSceneView.Repaint();
            ShowCommentInfo = false;
        }

        public static Color textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

        public static void Update()
        {

            if (showAreaComments != showAreaCommentsInternal)
            {
                var giz = FindObjectsOfType<BlockoutSceneViewCubeGizmo>().ToList();
                giz.ForEach(x => { x.commentsActive = showAreaComments; x.Update(); });
                if (SceneView.lastActiveSceneView)
                    SceneView.lastActiveSceneView.Repaint();
                showAreaCommentsInternal = showAreaComments;
            }
            if (showPinComments != showPinCommentsInternal)
            {
                var pins = FindObjectsOfType<BlockoutPinGizmo>().ToList();
                pins.ForEach(x => { x.commentsActive = showPinComments; x.Update(); });
                if (SceneView.lastActiveSceneView)
                    SceneView.lastActiveSceneView.Repaint();
                showPinCommentsInternal = showPinComments;
            }

            if (!ShowCommentInfo)
				return;
			
            var s = FindObjectsOfType<BlockoutSceneViewCubeGizmo>();
            var p = FindObjectsOfType<BlockoutPinGizmo>();
            List<GameObject> comments = s.ToList().Select(x => x.gameObject).ToList();
            comments.AddRange(p.ToList().Select(x => x.gameObject).ToList());

            if (generalNoteData == "")
                generalNoteData = "There are no general notes here.";
            if (toDoNoteData == "")
                toDoNoteData = "There are no TODO notes here.";
            if (otherNoteData == "")
                otherNoteData = "There are no other notes here.";

            targetName = "Global";
            List<Notepad> notes = Selection.gameObjects.ToList().Where(x => x.GetComponent<Notepad>() != null).Select(x => x.GetComponent<Notepad>()).ToList();
            if (notes.Count > 0)
            {
                targetName = notes[0].gameObject.name;
                if (notes[0].generalNotes != "")
                    generalNoteData = notes[0].generalNotes;
                else
                    generalNoteData = "There are no general notes.";
                if (notes[0].toDoNotes != "")
                    toDoNoteData = notes[0].toDoNotes;
                else
                    toDoNoteData = "There are no ToDo notes.";
                if (notes[0].otherNotes != "")
                    otherNoteData = notes[0].otherNotes;
                else
                    otherNoteData = "There are no other notes.";
            }
            else
            {
                Notepad targetNote = GlobalNotes;
                for (int t = 0; t < comments.Count; ++t)
                {
                    if (comments[t].GetComponent<Collider>())
                    {
                        if (comments[t].GetComponent<Collider>().bounds
                            .Contains(
                                SceneView.lastActiveSceneView.camera.ScreenToWorldPoint(new Vector3(0.5f, 0.5f, 0.15f)))
                        ) 
                        {
                            if (targetNote == GlobalNotes)
                                targetNote = comments[t].GetComponent<Notepad>();
                            else
                            {
                                if (Vector3.Distance(
                                        SceneView.lastActiveSceneView.camera.ScreenToWorldPoint(new Vector3(0.5f, 0.5f,
                                            0.15f)), targetNote.transform.position) >
                                    Vector3.Distance(
                                        SceneView.lastActiveSceneView.camera.ScreenToWorldPoint(new Vector3(0.5f, 0.5f,
                                            0.15f)), comments[t].transform.position)
                                )
                                {
                                    targetNote = comments[t].GetComponent<Notepad>();
                                }
                            }
                        }
                    }

                }

                if (targetNote != GlobalNotes)
                {
                    targetName = targetNote.gameObject.name;
                    
                }

                if(targetNote.generalNotes != "")
                    generalNoteData = targetNote.generalNotes;
                else
                    generalNoteData = "There are no general notes.";
                if (targetNote.toDoNotes != "")
                    toDoNoteData = targetNote.toDoNotes;
                else
                    toDoNoteData = "There are no ToDo notes.";
                if (targetNote.otherNotes != "")
                    otherNoteData = targetNote.otherNotes;
                else
                    otherNoteData = "There are no other notes.";
            }

            if (SceneView.lastActiveSceneView)
                camPos = SceneView.lastActiveSceneView.camera.ScreenToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
        }

        static float windowHeight = 150;

        static void DrawQuickPickerWindow(int windowID)
        {
            style.normal.textColor = textColor;

            using (new HorizontalCenteredScope(GUILayout.Width(260)))
            {
                using (new VerticalCenteredScope(GUILayout.Width(250)))
                {
                    GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
                    headerStyle.normal.textColor = Color.white;
                    
                    var generalHeader = new GUIContent("General");
                    var generalNoteContent= new GUIContent(generalNoteData);
                    var todoHeader = new GUIContent("ToDo");
                    var toDoContent = new GUIContent(toDoNoteData);
                    var otherHeader = new GUIContent("Other");
                    var othercontent = new GUIContent(otherNoteData);

                    GUILayout.Label(generalHeader, headerStyle);
                    GUILayout.Label(generalNoteContent, style);
                    GUILayout.Space(5);
                    GUILayout.Label(todoHeader, headerStyle);
                    GUILayout.Label(toDoContent, style);
                    GUILayout.Space(5);
                    GUILayout.Label(otherHeader, headerStyle);
                    GUILayout.Label(othercontent, style);

                    //windowHeight = GUILayoutUtility.GetRect(generalHeader, headerStyle).height +
                    //               GUILayoutUtility.GetRect(todoHeader, headerStyle).height +
                    //               GUILayoutUtility.GetRect(otherHeader, headerStyle).height +
                    //               GUILayoutUtility.GetRect(generalNoteContent, style).height +
                    //               GUILayoutUtility.GetRect(toDoContent, style).height +
                    //               GUILayoutUtility.GetRect(othercontent, style).height + 80;


                }
            }
            

        }

        static void DrawSceneCamera(int windowID)
        {
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.normal.textColor = textColor;
            using (new HorizontalCenteredScope())
            {
                using (new VerticalCenteredScope())
                {
                    GUILayout.Label(string.Format("X: {0:N1}   Y: {1:N1}   Z: {2:N1}", camPos.x, camPos.y, camPos.z), labelStyle,
                                    GUILayout.MaxWidth(220));
                }
            }
            
        }

        private static void OnScene(SceneView sceneview)
        {
            Update();

            if (!ShowCommentInfo) return;

           

            if (style == null)
            {
                style = new GUIStyle();
            }

           
            Handles.BeginGUI();

            float width = 260; 
            float hieght = windowHeight;

            Rect rect = new Rect(5, sceneview.position.height - hieght - 15, width, hieght);
            Rect camRect = new Rect(sceneview.position.width - width - 5, sceneview.position.height - 50 - 5, width, 50);

            GUI.skin.window.active = GUI.skin.window.normal;
            GUI.skin.window.onActive = GUI.skin.window.normal;
            GUI.skin.window.onNormal = GUI.skin.window.normal;

            GUILayout.Window(2, rect, DrawQuickPickerWindow, "Comment:" + targetName, GUI.skin.window);

            GUILayout.Window(3, camRect, DrawSceneCamera, "Scene Camera Position", GUI.skin.window);

            //GUILayout.BeginHorizontal();
            //GUILayout.Space(10);
            //GUILayout.BeginVertical();
            //GUILayout.FlexibleSpace();

            //GUI.contentColor = finalTextColor;
            //Color backgroundColor = GUI.backgroundColor;
            //backgroundColor.a = textColor.a;
            //GUI.backgroundColor = backgroundColor;

            //style.fontStyle = FontStyle.Bold;
            //GUILayout.Box(targetName, style, GUILayout.MaxWidth(260));
            //style.fontStyle = FontStyle.Normal;

            //GUILayout.Box(generalNoteData, style, GUILayout.MaxWidth(260));
            //GUILayout.Box(toDoNoteData, style, GUILayout.MaxWidth(260));
            //GUILayout.Box(otherNoteData, style, GUILayout.MaxWidth(260));

            //GUILayout.Space(25);
            //GUILayout.EndVertical();

            //GUILayout.FlexibleSpace();

            //GUILayout.BeginVertical();
            //GUILayout.FlexibleSpace();
            //style.alignment = TextAnchor.MiddleCenter;
            //GUILayout.Box(string.Format("X: {0:N1}   Y: {1:N1}   Z: {2:N1}", camPos.x, camPos.y, camPos.z), style,
            //    GUILayout.MaxWidth(220));
            //GUILayout.Space(25);
            //GUILayout.EndVertical();
            //GUILayout.Space(10);
            //GUILayout.EndHorizontal();

            Handles.EndGUI();
            
        }

        public static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

    }
}