/* Radical Forge Copyright (c) 2017 All Rights Reserved
   </copyright>
   <author>Frederic Babord</author>
   <date>15th June 2017</date>
   <summary>Hotkey support bindings for the Blockout Window</summary>*/

using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace RadicalForge.Blockout
{
    [InitializeOnLoad]
    public static class EditorHotkeysTracker
    {
        public static bool quickPicker = false, scrollPicker = false;
        static float amount = 0f;

        static float size;

        private static BlockoutEditorWindow targetEditor;
        


        private static void OnScene(SceneView view)
        {
            var e = Event.current;

            if (!targetEditor)
                targetEditor = EditorWindow.GetWindow<BlockoutEditorWindow>();

            if (e.control && e.alt && e.keyCode == KeyCode.B && e.type == EventType.KeyDown)
            {
                if (!BlockoutEditorWindow.isVisible)
                {
                    BlockoutEditorWindow.Init();
                    e.Use();
                }
                else
                {
                    targetEditor.Close();
                    SceneView.currentDrawingSceneView.Focus();
                    e.Use();
                }
            }

            if (!targetEditor)
                return;


            if (e != null && e.delta != Vector2.zero && e.type == EventType.ScrollWheel && scrollPicker)
            {
                SceneView.lastActiveSceneView.size = size;
                amount += e.delta.y;
                if (amount >= 5.0f && targetEditor.suggestedSection.shownItems.Count > 0)
                {
                    int first = targetEditor.suggestedSection.shownItems[0];
                    targetEditor.suggestedSection.shownItems.RemoveAt(0);
                    targetEditor.suggestedSection.shownItems.Add(first);
                    amount -= 5f;
                }
                else if (amount <= -5f && targetEditor.suggestedSection.shownItems.Count > 0)
                {
                    int last =
                        targetEditor.suggestedSection
                                    .shownItems[targetEditor.suggestedSection.shownItems.Count - 1];
                    targetEditor.suggestedSection.shownItems
                                .RemoveAt(targetEditor.suggestedSection.shownItems.Count - 1);
                    targetEditor.suggestedSection.shownItems.Insert(0, last);
                    amount += 5f;
                }
                e.Use();
                SceneView.lastActiveSceneView.Repaint();

                
            }

            else if (e != null && e.keyCode != KeyCode.None)
            {

                // Snapping Toggle
                if (e.alt && e.keyCode == KeyCode.S && e.type == EventType.KeyDown)
                {
                    if (BlockoutEditorWindow.isVisible)
                    {
                        BlockoutEditorSettings.AutoSnap = !BlockoutEditorSettings.AutoSnap;
                        targetEditor.Repaint();
                        SceneView.currentDrawingSceneView.Focus();
                        e.Use();
                    }
                }
                // Increase snap value
                else if (e.alt && e.control && e.keyCode == KeyCode.Z && e.type == EventType.KeyDown)
                {
                    if (BlockoutEditorWindow.isVisible)
                    {
                        var window = targetEditor;
                        window.gridSnapping.IncreaseSnapValue();
                        targetEditor.Repaint();
                        SceneView.currentDrawingSceneView.Focus();
                        e.Use();
                    }
                }
                // Decrease Snap Value
                else if (e.alt && e.control && e.keyCode == KeyCode.X && e.type == EventType.KeyDown)
                {
                    if (BlockoutEditorWindow.isVisible)
                    {
                        var window = targetEditor;
                        window.gridSnapping.DecreaseSnapValue();
                        targetEditor.Repaint();
                        SceneView.currentDrawingSceneView.Focus();
                        e.Use();
                    }
                }
                // Snap To Floor
                else if (e.keyCode == KeyCode.End && e.type == EventType.KeyDown)
                {
                    var tempDoSnap = BlockoutEditorSettings.AutoSnap;
                    if (tempDoSnap)
                        BlockoutEditorSettings.AutoSnap = !BlockoutEditorSettings.AutoSnap;
                    BlockoutStaticFunctions.Snap(Vector3.down, BlockoutAxis.Y, true);
                    if (tempDoSnap)
                        BlockoutEditorSettings.AutoSnap = !BlockoutEditorSettings.AutoSnap;
                    e.Use();
                }
                // Toggle Comments
                else if (e.alt && e.keyCode == KeyCode.C && e.type == EventType.KeyDown)
                {
                    if (BlockoutEditorWindow.isVisible)
                    {
                        var window = targetEditor;
                        window.commentsSection.showSection = !window.commentsSection.showSection;
                        if (window.commentsSection.showSection)
                            window.commentsSection.showSceneInformation = true;
                        SceneView.currentDrawingSceneView.Focus();
                        e.Use();
                    }
                }
                // Initialise Quick Picker
                else if (e.keyCode == KeyCode.C && e.type == EventType.KeyDown && !scrollPicker)
                {
                    if (Selection.activeGameObject)
                    {
                        scrollPicker = true;
                        size = SceneView.lastActiveSceneView.size;
                        Selection.activeGameObject.GetComponent<Collider>().enabled = false;
                        targetEditor.suggestedSection.InitialiseScrollPicker();
                    }
                    e.Use();
                }
                else if ( e.keyCode == KeyCode.Space && e.type == EventType.KeyDown)
                {
                    if (!quickPicker && !scrollPicker)
                    {
                        quickPicker = true;
                        targetEditor.suggestedSection.InitialiseQuickPicker();
                        e.Use();
                    }
                    else if (scrollPicker)
                    {
                        scrollPicker = false;
                        targetEditor.suggestedSection.ConfirmSpawn();
                        e.Use();
                    }
                }
                else if (e.keyCode == KeyCode.Escape && e.type == EventType.KeyDown && quickPicker)
                {
                    
                    quickPicker = false;
                    targetEditor.suggestedSection.CancelQickpicker();
                    e.Use();
                }
                else if (e.keyCode == KeyCode.C && e.type == EventType.KeyUp && scrollPicker)
                {
                    scrollPicker = false;
                    targetEditor.suggestedSection.ConfirmSwap();
                    e.Use();
                }
                else if (e.keyCode == KeyCode.Space && e.type == EventType.KeyUp && scrollPicker && !quickPicker)
                {
                    scrollPicker = false;
                    targetEditor.suggestedSection.ConfirmSwap();
                    e.Use();
                }
                else if (e.alt && e.keyCode == KeyCode.Space && e.type == EventType.KeyUp && scrollPicker)
                {
                    scrollPicker = false;
                    targetEditor.suggestedSection.ConfirmSpawn();
                    e.Use();
                }
                // Select asset
                else if (e.keyCode == KeyCode.G && e.type == EventType.KeyDown)
                {
                    BlockoutStaticFunctions.SelectAsset();
                    e.Use();
                }
                // Lock selected
                else if (e.keyCode == KeyCode.L && e.type == EventType.KeyDown)
                {
                    Selection.gameObjects.Where(x => x.GetComponent<BlockoutHelper>())
                             .Select(x => x.GetComponent<BlockoutHelper>()).ToList()
                             .ForEach(x => x.SetLock(!x.Locked));
                    e.Use();
                }

                

                if (quickPicker)
                {
                    if (e.keyCode == KeyCode.LeftArrow && e.type == EventType.KeyDown)
                    {
                        targetEditor.suggestedSection.selected--;
                        targetEditor.suggestedSection.scroll.y =
                            (float)targetEditor.suggestedSection.selected / (float)targetEditor.suggestedSection.SuggestedItemCount;
                        e.Use();
                    }
                    else if (e.keyCode == KeyCode.RightArrow && e.type == EventType.KeyDown)
                    {
                        targetEditor.suggestedSection.selected++;
                        targetEditor.suggestedSection.scroll.y =
                            (float)targetEditor.suggestedSection.selected / (float)targetEditor.suggestedSection.SuggestedItemCount;
                        e.Use();
                    }
                    else if (e.keyCode == KeyCode.UpArrow && e.type == EventType.KeyDown)
                    {
                        targetEditor.suggestedSection.selected -= targetEditor.suggestedSection.maxPerRow;
                        targetEditor.suggestedSection.scroll.y =
                            (float)targetEditor.suggestedSection.selected / (float)targetEditor.suggestedSection.SuggestedItemCount;
                        e.Use();
                    }
                    else if (e.keyCode == KeyCode.DownArrow && e.type == EventType.KeyDown)
                    {
                        targetEditor.suggestedSection.selected += targetEditor.suggestedSection.maxPerRow;
                        targetEditor.suggestedSection.scroll.y =
                            (float)targetEditor.suggestedSection.selected / (float)targetEditor.suggestedSection.SuggestedItemCount;
                        e.Use();
                    }

                    else if ((e.keyCode == KeyCode.Space || e.keyCode == KeyCode.Return) && e.type == EventType.KeyDown)
                    {
                        targetEditor.suggestedSection.SpawnSelected(targetEditor.suggestedSection.selected);
                        targetEditor.suggestedSection.scroll.y =
                            (float)targetEditor.suggestedSection.selected / (float)targetEditor.suggestedSection.SuggestedItemCount;
                        e.Use();
                    }
                    else if (e.keyCode == KeyCode.Escape && e.type == EventType.KeyDown)
                    {
                        targetEditor.suggestedSection.CancelQickpicker();
                        e.Use();
                    }
                }
            }
            
        }

        public static void Init(BlockoutEditorWindow targetEditor_)
        {
            targetEditor = targetEditor_;
            SceneView.onSceneGUIDelegate += OnScene;
        }

        public static void Destroy()
        {
            SceneView.onSceneGUIDelegate -= OnScene;
        }
    }
}