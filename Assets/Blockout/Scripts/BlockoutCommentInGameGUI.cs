/* Radical Forge Copyright (c) 2017 All Rights Reserved
   </copyright>
   <author>Frederic Babord</author>
   <date>15th June 2017</date>
   <summary>Game mode comment helper</summary>*/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RadicalForge.Blockout
{

    public class BlockoutCommentInGameGUI : MonoBehaviour
    {
        private static BlockoutCommentInGameGUI instance;

        public static BlockoutCommentInGameGUI Instance
        {
            get
            {
                if (!instance)
                    instance = FindObjectOfType<BlockoutCommentInGameGUI>();
                return instance;
            }
        }
        public KeyCode inGameToggle = KeyCode.F12;
        public KeyCode pinGameToggle = KeyCode.F11;
        public KeyCode areaGameToggle = KeyCode.F10;

        private List<BlockoutSceneViewCubeGizmo> areaComments;
        private List<BlockoutPinGizmo> pinComments;

        public bool intialVisabilityStateArea = false, intialVisabilityStatePin = false;


        private static string targetName = "";
        private static string generalNoteData = "";
        private static string toDoNoteData = "";
        private static string otherNoteData = "";
        private static GUIStyle style;
        private static Vector3 camPos = Vector3.zero;
        public Color textColor = new Color(1, 1, 1, 0.5f);
        private bool currentGlobalState = true;

        public GameObject target;

        public Notepad GlobalNotes;

        void Awake()
        {
            if(instance)
                Destroy(this);
            else
                instance = this;
        }

        // Use this for initialization
        void Start()
        {
            areaComments = FindObjectsOfType<BlockoutSceneViewCubeGizmo>().ToList();
            pinComments = FindObjectsOfType<BlockoutPinGizmo>().ToList();
            Set(intialVisabilityStateArea, intialVisabilityStatePin);

            if(!target)
                target = GameObject.FindGameObjectWithTag("Player");
            if (!target && Camera.current)
                target = Camera.current.transform.gameObject;
        }

        // Update is called once per frame
        void Update()
        {
            if (!target)
                target = GameObject.FindGameObjectWithTag("Player");
            if (!target && Camera.current)
                target = Camera.current.transform.gameObject;

            if (Input.GetKeyDown(inGameToggle))
            {
                Toggle();
            }

            if (Input.GetKeyDown(pinGameToggle))
            {
                TogglePin();
            }

            if (Input.GetKeyDown(areaGameToggle))
            {
                ToggleArea();
            }

            // Set GUI data
            if (intialVisabilityStateArea || intialVisabilityStatePin)
            {
                var s = FindObjectsOfType<BlockoutSceneViewCubeGizmo>();
                var p = FindObjectsOfType<BlockoutPinGizmo>();
                List<GameObject> comments = new List<GameObject>();

                if(intialVisabilityStateArea)
                    comments.AddRange(s.ToList().Select(x => x.gameObject).ToList());
                if (intialVisabilityStatePin)
                    comments.AddRange(p.ToList().Select(x => x.gameObject).ToList());
                
                generalNoteData = "";
                toDoNoteData = "";
                otherNoteData = "";

                targetName = SceneManager.GetActiveScene().name;

                Notepad targetNote = GlobalNotes;
                for (int t = 0; t < comments.Count; ++t)
                {
                    if (comments[t].GetComponent<Collider>())
                    {
                        if (comments[t].GetComponent<Collider>().bounds
                            .Contains(
                                target.transform.position)
                        )
                        {
                            if (targetNote == null)
                                targetNote = comments[t].GetComponent<Notepad>();
                            else
                            {
                                if (Vector3.Distance(
                                        target.transform.position, targetNote.transform.position) >
                                    Vector3.Distance(
                                        target.transform.position, comments[t].transform.position)
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

                if (targetNote.generalNotes != "")
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




                camPos = target.transform.position;
            }
        }

        // Legacy draw comments in game
        void OnGUI()
        {
            if (currentGlobalState)
            {
                if (style == null)
                {
                    style = new GUIStyle(GUI.skin.box);
                }

                style.normal.textColor = textColor;
                Color finalTextColor = textColor;
                finalTextColor.a = 1.0f;
                style.normal.textColor = finalTextColor;
                style.alignment = TextAnchor.MiddleLeft;

                GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));


                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();

                GUI.contentColor = finalTextColor;
                Color backgroundColor = GUI.backgroundColor;
                backgroundColor.a = textColor.a;
                GUI.backgroundColor = backgroundColor;

                style.fontStyle = FontStyle.Bold;
                GUILayout.Box(targetName, style, GUILayout.MaxWidth(260));
                style.fontStyle = FontStyle.Normal;

                GUILayout.Box(generalNoteData, style, GUILayout.MaxWidth(260));
                GUILayout.Box(toDoNoteData, style, GUILayout.MaxWidth(260));
                GUILayout.Box(otherNoteData, style, GUILayout.MaxWidth(260));

                GUILayout.Space(10);
                GUILayout.EndVertical();

                GUILayout.FlexibleSpace();

                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                style.alignment = TextAnchor.MiddleCenter;
                GUILayout.Box(string.Format("X: {0:N1}   Y: {1:N1}   Z: {2:N1}", camPos.x, camPos.y, camPos.z), style,
                    GUILayout.MaxWidth(220));
                GUILayout.Space(10);
                GUILayout.EndVertical();

                GUILayout.Space(10);
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                
                GUILayout.EndArea();
            }
        }

        // Toggle All Comments
        public void Toggle()
        {
            currentGlobalState = !currentGlobalState;

            Set(currentGlobalState, currentGlobalState);
        }

        // Toggle area comments
        public void ToggleArea()
        {
            intialVisabilityStateArea = !intialVisabilityStateArea;
            areaComments.ForEach(x =>
            {
                x.commentsActive = intialVisabilityStateArea;
            });
            currentGlobalState = (intialVisabilityStateArea || intialVisabilityStatePin);
        }

        // Toggle pin comments
        public void TogglePin()
        {
            
            intialVisabilityStatePin = !intialVisabilityStatePin;
            pinComments.ForEach(x =>
            {
                x.commentsActive = intialVisabilityStatePin;
            });

            currentGlobalState = (intialVisabilityStateArea || intialVisabilityStatePin);
        }

        // Internally set area and pin comments
        void Set(bool areaValue, bool pinValue)
        {
            areaComments.ForEach(x =>
            {
                x.SetActive(areaValue);
            });
            pinComments.ForEach(x =>
            {
                x.SetActive(pinValue);
            });
            intialVisabilityStateArea = areaValue;
            intialVisabilityStatePin = pinValue;
            currentGlobalState = (intialVisabilityStateArea || intialVisabilityStatePin);
        }
    }

}