using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RadicalForge.Blockout
{

    public class CommentExplorerWindowTab
    {
        private List<Notepad> notepads = new List<Notepad>();

        public Color oddRowColor = new Color(0.709f, 0.709f, 0.709f);
        public Color evenRowColor = new Color(0.670f, 0.670f, 0.670f);
        public Color selectedColor;
        

        Vector2 scrollView, previousMosePos;

        private int nameWidth = 150, colorWidth = 80, generalNotesWidth = 350, todoNotesWidth = 350, otherNotesWidth = 350;
        private bool resize;
        private ResizeOption resizeOption = ResizeOption.Null;

        private int selectedItem = -1;



        enum ResizeOption
        {
            Name,
            Color,
            General,
            ToDo,
            Other,
            Null
        }

        public enum  TabType
        {
            AreaComment,
            PinComment,
            All
        }
        

        public CommentExplorerWindowTab InitTab(TabType tabType)
        {
            if (EditorGUIUtility.isProSkin)
            {
                oddRowColor = new Color(0.249f, 0.249f, 0.249f);
                evenRowColor = new Color(0.19f, 0.19f, 0.19f);
            }

            notepads.Clear();
            if (GameObject.Find("Blockout"))
            {
                notepads.Add(GameObject.Find("Blockout").GetComponent<Notepad>());
                if (tabType == TabType.AreaComment)
                    notepads.AddRange(Object.FindObjectsOfType<BlockoutSceneViewCubeGizmo>()
                                            .Select(x => x.GetComponent<Notepad>()).ToArray());
                else if (tabType == TabType.PinComment)
                    notepads.AddRange(Object.FindObjectsOfType<BlockoutPinGizmo>()
                                            .Select(x => x.GetComponent<Notepad>())
                                            .ToArray());
                else
                    notepads.AddRange(Object.FindObjectsOfType<Notepad>().Where(x => x.transform.parent != null)
                                            .ToArray());
            }
            else
                Debug.LogError("Requires a Blockout scene!");

            return this;
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        void DrawResizeHandle(ResizeOption targetOption)
        {
            GUILayout.Box(EUIResourceManager.Instance.GetTexture("Blockout_Icon_Spacer"), EUIResourceManager.Instance.Skin.GetStyle("Texture"), GUILayout.MaxHeight(25), GUILayout.Width(10));
            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.ResizeHorizontal);
            if (Event.current.type == EventType.MouseDown && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                resize = true;
                resizeOption = targetOption;
                previousMosePos = Event.current.mousePosition;
            }
        }

        public void DrawTable(CommentExplorer owningExplorer, TabType tabType, string searchString = "", float windowWidth = 150)
        {

            Notepad[] targetNotepads;
            if (searchString != "")
            {
                targetNotepads = notepads.Count(x => x.gameObject.name.Contains(searchString)) > 0 ? notepads.Where(x => x.gameObject.name.Contains(searchString)).ToArray() : notepads.ToArray();
                // // TODO: Bug here
                // var genNotes = notepads.Count(x => x.generalNotes.Contains(searchString)) > 0 ? notepads.Where(x => x.generalNotes.Contains(searchString)).ToArray() : new Notepad[]{};
                // var otherNotes = notepads.Count(x => x.otherNotes.Contains(searchString)) > 0 ? notepads.Where(x => x.otherNotes.Contains(searchString)).ToArray() : new Notepad[]{};
                // var todoNotes = notepads.Count(x => x.toDoNotes.Contains(searchString)) > 0 ? notepads.Where(x => x.toDoNotes.Contains(searchString)).ToArray() : new Notepad[]{};

                // var finalNotetes = targetNotepads.Concat(genNotes).Concat(otherNotes).Concat(todoNotes).ToArray();
                // targetNotepads = finalNotetes;

                // if(notepads.Count(x => x.generalNotes.Contains(searchString)) > 0)
                //     targetNotepads = targetNotepads.Concat(notepads.Where(x => x.generalNotes.Contains(searchString) && !targetNotepads.Contains(x)).ToArray()).ToArray();
                // if(notepads.Count(x => x.otherNotes.Contains(searchString)) > 0)
                //     targetNotepads = targetNotepads.Concat(notepads.Where(x => x.otherNotes.Contains(searchString) && !targetNotepads.Contains(x)).ToArray()).ToArray();
                // if(notepads.Count(x => x.toDoNotes.Contains(searchString)) > 0)
                //     targetNotepads = targetNotepads.Concat(notepads.Where(x => x.toDoNotes.Contains(searchString) && !targetNotepads.Contains(x)).ToArray()).ToArray();
            }
            else
                targetNotepads = notepads.ToArray();

            selectedColor = GUI.skin.settings.selectionColor;

            using (var sv = new EditorGUILayout.ScrollViewScope(scrollView, GUILayout.MaxWidth(windowWidth - 30)))
            {
                scrollView = sv.scrollPosition;

                GUILayout.BeginVertical();

                GUILayout.BeginHorizontal();
                GUILayout.Label(EUIResourceManager.Instance.GetTexture("Blockout_Name"), GUILayout.Width(nameWidth), GUILayout.MaxHeight(25));

                DrawResizeHandle(ResizeOption.Name);
                
                GUILayout.Label(EUIResourceManager.Instance.GetTexture("Blockout_Color"), GUILayout.Width(colorWidth), GUILayout.MaxHeight(25));
                DrawResizeHandle(ResizeOption.Color);
                GUILayout.Label(EUIResourceManager.Instance.GetTexture("Blockout_General_Notes"),  GUILayout.Width(generalNotesWidth), GUILayout.MaxHeight(25));
                DrawResizeHandle(ResizeOption.General);
                GUILayout.Label(EUIResourceManager.Instance.GetTexture("Blockout_To_Do_Notes"),GUILayout.Width(todoNotesWidth), GUILayout.MaxHeight(25));
                DrawResizeHandle(ResizeOption.ToDo);
                GUILayout.Label(EUIResourceManager.Instance.GetTexture("Blockout_Other_Notes"), GUILayout.Width(otherNotesWidth), GUILayout.MaxHeight(25));
                DrawResizeHandle(ResizeOption.Other);
                GUILayout.EndHorizontal();

                GUILayout.Space(5);

                
                if (resize)
                {
                    switch (resizeOption)
                    {
                        case ResizeOption.Name:
                            nameWidth -= (int)(previousMosePos - Event.current.mousePosition).x;
                            previousMosePos = Event.current.mousePosition;
                            nameWidth = Mathf.Clamp(nameWidth, 20, 400);
                            break;
                        case ResizeOption.Color:
                            colorWidth -= (int)(previousMosePos - Event.current.mousePosition).x;
                            previousMosePos = Event.current.mousePosition;
                            colorWidth = Mathf.Clamp(colorWidth, 20, 400);
                            break;
                        case ResizeOption.General:
                            generalNotesWidth -= (int)(previousMosePos - Event.current.mousePosition).x;
                            previousMosePos = Event.current.mousePosition;
                            generalNotesWidth = Mathf.Clamp(generalNotesWidth, 100, 800);
                            break;
                        case ResizeOption.ToDo:
                            todoNotesWidth -= (int)(previousMosePos - Event.current.mousePosition).x;
                            previousMosePos = Event.current.mousePosition;
                            todoNotesWidth = Mathf.Clamp(todoNotesWidth, 100, 800);
                            break;
                        case ResizeOption.Other:
                            otherNotesWidth -= (int)(previousMosePos - Event.current.mousePosition).x;
                            previousMosePos = Event.current.mousePosition;
                            otherNotesWidth = Mathf.Clamp(otherNotesWidth, 100, 800);
                            break;
                    }

                    owningExplorer.Repaint();
                }
                if (Event.current.rawType == EventType.MouseUp)
                {
                    resize = false;
                }



                GUIStyle gsTest = new GUIStyle();




                for (int i = 0; i < targetNotepads.Length; ++i)
                {


                    gsTest.normal.background = selectedItem == i ? MakeTex(1, 1, selectedColor) : MakeTex(1, 1, i % 2 == 0 ? oddRowColor : evenRowColor);

                    using (var he = new EditorGUILayout.HorizontalScope(gsTest))
                    {

                        using (new VerticalCenteredScope(GUILayout.Height(EditorGUIUtility.singleLineHeight * 3)))
                        {
                            GUILayout.Label(targetNotepads[i].gameObject.name, EditorStyles.boldLabel,
                                            GUILayout.Width(nameWidth));
                        }
                        GUILayout.Space(8);

                        if (targetNotepads[i].GetComponent<BlockoutSceneViewCubeGizmo>())
                            targetNotepads[i].GetComponent<BlockoutSceneViewCubeGizmo>().volumeColor =
                                EditorGUILayout.ColorField(
                                    targetNotepads[i].GetComponent<BlockoutSceneViewCubeGizmo>().volumeColor,
                                    GUILayout.Width(colorWidth));
                        else if (targetNotepads[i].GetComponent<BlockoutPinGizmo>())
                            targetNotepads[i].GetComponent<BlockoutPinGizmo>().volumeColor =
                                EditorGUILayout.ColorField(
                                                           targetNotepads[i]
                                                               .GetComponent<BlockoutPinGizmo>().volumeColor,
                                                           GUILayout.Width(colorWidth));
                        else
                            GUILayout.Space(colorWidth+3);
                        GUILayout.Space(10);
                        targetNotepads[i].generalNotes = GUILayout.TextArea(targetNotepads[i].generalNotes,
                            GUILayout.Width(generalNotesWidth),
                            GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 3));
                        GUILayout.Space(10);
                        targetNotepads[i].toDoNotes = GUILayout.TextArea(targetNotepads[i].toDoNotes,
                            GUILayout.Width(todoNotesWidth),
                            GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 3));
                        GUILayout.Space(10);
                        targetNotepads[i].otherNotes = GUILayout.TextArea(targetNotepads[i].otherNotes,
                            GUILayout.Width(otherNotesWidth),
                            GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 3));

                        if (Event.current.type == EventType.MouseDown &&
                            he.rect.Contains(Event.current.mousePosition))
                        {
                            selectedItem = i;
                            Selection.activeGameObject = notepads[i].gameObject;
                            owningExplorer.Repaint();
                        }

                    }

                    if(i == 0)
                        GUILayout.Space(10);

                }


                GUILayout.EndVertical();
            }

        }
    }

}