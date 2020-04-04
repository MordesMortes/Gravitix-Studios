/* Radical Forge Copyright (c) 2017 All Rights Reserved
   </copyright>
   <author>Frederic Babord</author>
   <date>15th June 2017</date>
   <summary>Editor Helper to place pin comments in the scene</summary>*/

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RadicalForge.Blockout
{
    [InitializeOnLoad]
    [ExecuteInEditMode]
    public class BlockoutEditorHelper : EditorWindow
    {
        private static BlockoutEditorWindow targetWindow;

        public static void Awake(BlockoutEditorWindow targetWindow_)
        {
            targetWindow = targetWindow_;
            SceneView.onSceneGUIDelegate += OnScene;
        }

        public static void Destroy()
        {
            SceneView.onSceneGUIDelegate -= OnScene;
        }

        static GameObject spawnedRuler;

        private static void OnScene(SceneView sceneview)
        {
            if (targetWindow)
            {
                if (targetWindow.commentsSection.commentPinToPlace >= 0)
                {
                    var cur = Event.current;

                    if ((cur.type == EventType.MouseDown || cur.type == EventType.MouseUp) && cur.button == 0)
                    {
                        var hits = Physics.SphereCastAll(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), 0.25f, 50);
                        if (hits.Length > 0)
                            for (var i = 0; i < hits.Length; i++)
                            {
                                var hit = hits[i];

                                if (hit.collider.GetComponent<Notepad>())
                                    continue;

                                var targetRotation =
                                    (Quaternion.FromToRotation(Vector3.up, hit.normal) *
                                     hit.collider.transform.rotation)
                                    .eulerAngles;

                                for (var j = 0; j < 3; ++j)
                                    if (targetRotation[j] > -45 && targetRotation[j] <= 45)
                                        targetRotation[j] = 0;
                                    else if (targetRotation[j] > 45 && targetRotation[j] <= 135)
                                        targetRotation[j] = 90;
                                    else if (targetRotation[j] > 135 && targetRotation[j] <= 225)
                                        targetRotation[j] = 180;
                                    else
                                        targetRotation[j] = 270;

                                targetRotation[0] += 180;

                                var pin = Instantiate(targetWindow.commentsSection.pinObjects[targetWindow.commentsSection.commentPinToPlace],
                                                      hit.point,
                                                      Quaternion.Euler(targetRotation));
                                Undo.RegisterCreatedObjectUndo(pin, "Create Pin Comment");
                                pin.transform.SetParent(GameObject.Find("Blockout").GetComponentsInChildren<BlockoutSection>().Where(x => x.Section == SectionID.Comments).ToArray()[0].transform, true);
                                pin.AddComponent<Notepad>();
                                var bpg = pin.AddComponent<BlockoutPinGizmo>();
                                bpg.SelectAfterFrame = true;
                                targetWindow.commentsSection.commentPinToPlace = -1;
                                cur.Use();
                                break;
                            }
                    }
                }
                else if (targetWindow.othersSection.rulerObjectToPlace > 0)
                {
                    var cur = Event.current;

                    if ((cur.type == EventType.MouseDown || cur.type == EventType.MouseUp) && cur.button == 0)
                    {
                        var hits = Physics.SphereCastAll(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), 0.25f, 50);

                        if (hits.Length > 0)
                            for (var i = 0; i < hits.Length; i++)
                            {
                                var hit = hits[i];

                                if (hit.collider.GetComponent<Notepad>())
                                    continue;

                                var targetRotation =
                                    (Quaternion.FromToRotation(Vector3.up, hit.normal) *
                                     hit.collider.transform.rotation)
                                    .eulerAngles;

                                for (var j = 0; j < 3; ++j)
                                    if (targetRotation[j] > -45 && targetRotation[j] <= 45)
                                        targetRotation[j] = 0;
                                    else if (targetRotation[j] > 45 && targetRotation[j] <= 135)
                                        targetRotation[j] = 90;
                                    else if (targetRotation[j] > 135 && targetRotation[j] <= 225)
                                        targetRotation[j] = 180;
                                    else
                                        targetRotation[j] = 270;

                                if (targetWindow.othersSection.rulerObjectToPlace == 1)
                                {
                                    spawnedRuler = Instantiate(targetWindow.othersSection.rulerPrefab, hit.point, Quaternion.identity);
                                    Selection.activeGameObject = spawnedRuler;
                                    targetWindow.othersSection.previousRuler = spawnedRuler.GetComponent<BlockoutRulerObject>();
                                    spawnedRuler.name = "Blockout Ruler";
                                    spawnedRuler.GetComponent<LineRenderer>().SetPositions(new []{Vector3.zero, Vector3.zero});
                                    spawnedRuler.transform.GetChild(0).transform.localPosition = Vector3.zero;
                                    spawnedRuler.transform.GetChild(1).gameObject.SetActive(false);
                                    spawnedRuler.transform.GetChild(1).rotation = Quaternion.Euler(targetRotation);
                                    spawnedRuler.GetComponentInChildren<TextMesh>().gameObject.SetActive(false);
                                    targetWindow.othersSection.rulerObjectToPlace++;
                                    cur.Use();
                                    targetWindow.Repaint();
                                }
                                else if (targetWindow.othersSection.rulerObjectToPlace == 2)
                                {
                                    spawnedRuler.transform.GetChild(1).gameObject.SetActive(true);
                                    spawnedRuler.transform.GetChild(1).position = hit.point;
                                    spawnedRuler.transform.GetChild(1).rotation = Quaternion.Euler(targetRotation);
                                    spawnedRuler.GetComponentInChildren<TextMesh>(true).gameObject.SetActive(true);
                                    targetWindow.othersSection.rulerObjectToPlace = 0;
                                    Selection.activeGameObject = spawnedRuler;
                                    cur.Use();
                                    targetWindow.Repaint();
                                }
                                
                                break;
                            }
                    }
                }
            }
        }
    }
}