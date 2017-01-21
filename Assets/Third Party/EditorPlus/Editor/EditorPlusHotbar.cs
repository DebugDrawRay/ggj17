using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#pragma warning disable 618

namespace EditorPlus
{
    public class EditorPlusHotbar : EditorWindow, IHasCustomMenu
    {
        private bool init = false;

        [SerializeField]
        private WidgetHost widgetHost;

        private Object selectedObj;

        [SerializeField]
        private Vector2 scrollviewPosition = Vector2.zero;

        [SerializeField]
        private int textLimit = 3;

        [SerializeField]
        private bool useTooltip = true;

        [SerializeField]
        public bool useAutoSave = true;

        private static Vector2 StartMargin = new Vector2(6f, 6f);

        private static Vector2 ButtonMargin = new Vector2(5f, 5f);

        private static Vector2 ButtonSize = new Vector2(55f, 55f);

        private static GUISkin skin;

        private static List<string> savedHotbars;

        [SerializeField]
        public List<Object> hotbarObjects = new List<Object>();

        public Dictionary<Object, Texture2D> hotbarPreviewCache = new Dictionary<Object, Texture2D>();


        [MenuItem("Window/EditorPlus/Hotbar")]
        public static void Init()
        {
            EditorPlusHotbar w = CreateInstance<EditorPlusHotbar>();
            //window = (EditorPlusHotbar)EditorWindow.GetWindow(typeof(EditorPlusHotbar));
            w.title = "Hotbar";
            w.LoadHotbar("Hotbar");
            w.minSize = new Vector2(StartMargin.x + ButtonSize.x + ButtonMargin.x, StartMargin.y + ButtonSize.y + ButtonMargin.y);

            w.Show();
        }


        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
        }


        void ShowButton(Rect position)
        {
            Event e = Event.current;
            if (GUI.Button(position, EditorPlus.EditorPlusIcon, new GUIStyle(GUI.skin.label)))
            {
                if (e.button != 0)
                    return;
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("Text Limit/Short Text"), (textLimit == 3), SetTextLimit, 3);
                menu.AddItem(new GUIContent("Text Limit/Full Text"), (textLimit == int.MaxValue), SetTextLimit, int.MaxValue);
                menu.AddItem(new GUIContent("Text Limit/No Text"), (textLimit == 0), SetTextLimit, 0);

                //menu.AddSeparator("Scale");
                menu.AddItem(new GUIContent("Scale/0.5"), (widgetHost.WidgetScale == 0.5f), ChangeScale, 0.5f);
                menu.AddItem(new GUIContent("Scale/0.75"), (widgetHost.WidgetScale == 0.75f), ChangeScale, 0.75f);
                menu.AddItem(new GUIContent("Scale/1.0"), (widgetHost.WidgetScale == 1f), ChangeScale, 1f);
                menu.AddItem(new GUIContent("Scale/1.25"), (widgetHost.WidgetScale == 1.25f), ChangeScale, 1.25f);
                menu.AddItem(new GUIContent("Scale/1.5"), (widgetHost.WidgetScale == 1.5f), ChangeScale, 1.5f);
                menu.AddItem(new GUIContent("Scale/2"), (widgetHost.WidgetScale == 2f), ChangeScale, 2f);
                menu.AddItem(new GUIContent("Scale/3"), (widgetHost.WidgetScale == 3f), ChangeScale, 3f);

                menu.AddItem(new GUIContent("Tooltip/Show"), useTooltip, SetTooltipMode, true);
                menu.AddItem(new GUIContent("Tooltip/Hide"), !useTooltip, SetTooltipMode, false);

                menu.AddSeparator("");
                if (savedHotbars.Count == 0)
                {
                    menu.AddDisabledItem(new GUIContent("Saving/Load"));
                }
                else
                {
                    foreach (string bar in savedHotbars)
                    {
                        menu.AddItem(new GUIContent("Saving/Load/" + bar), false, LoadHotbar, bar);
                    }
                }

                menu.AddItem(new GUIContent("Saving/Save"), false, SaveHotbar);

                menu.AddItem(new GUIContent("Saving/Autosave/On"), useAutoSave, SetAutoSave, true);

                menu.AddItem(new GUIContent("Saving/Autosave/Off"), !useAutoSave, SetAutoSave, false);

                menu.AddItem(new GUIContent("Saving/Clear Saves"), false, ClearAllSaves);

                menu.AddSeparator("");

                menu.AddItem(new GUIContent("Rename"), false, EditorPlusTitleWindow.Init, this);

                menu.AddItem(new GUIContent("Clear Bar"), false, ClearHotbar);


                menu.ShowAsContext();
            }
        }


        void OnEnable()
        {
            if(widgetHost == null)
                widgetHost = new WidgetHost(position, StartMargin, ButtonMargin, ButtonSize, 0, 1f, false);

            if (skin == null)
                skin = AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath +"HotbarSkin.guiskin") as GUISkin;

            if (savedHotbars == null)
            {
                savedHotbars = new List<string>();
                string bars = EditorPrefs.GetString("EditorPlus.Hotbar.Barnames", "");
                if(bars != "")
                {
                    savedHotbars.AddRange(bars.Split(','));
                }
            }

            init = true;
        }


        void OnSelectionChange()
        {
            if(hotbarObjects.Contains(Selection.activeObject))
            {
                Texture2D preview = AssetPreview.GetAssetPreview(Selection.activeObject);
                if (preview != null)
                {
                    if (hotbarPreviewCache.ContainsKey(Selection.activeObject))
                        hotbarPreviewCache[Selection.activeObject] = preview;
                    else
                        hotbarPreviewCache.Add(Selection.activeObject, preview);
                }
            }

            if (Selection.activeObject != selectedObj)
            {
                selectedObj = null;
                if (!hotbarObjects.Contains(Selection.activeObject))
                    Repaint();
            }
        }


        public void ChangeScale(object scale)
        {
            widgetHost.WidgetScale = (float)scale;
        }


        public void SetWindowTitle(object name)
        {
            title = (string)name;
        }


        public void SetTextLimit(object limit)
        {
            textLimit = (int)limit;
        }


        public void SetTooltipMode(object tltp)
        {
            useTooltip = (bool)tltp;
        }


        public void SetAutoSave(object save)
        {
            useAutoSave = (bool)save;
        }


        void OnGUI()
        {
            if (init == false)
                return;

            if (GUI.skin != skin)
                GUI.skin = skin;

            Event e = Event.current;

            bool insideWindow = false;

            if (e.mousePosition.x > 0f && e.mousePosition.x < position.width && e.mousePosition.y > 0f && e.mousePosition.y < position.height && (DragAndDrop.objectReferences.Length > 0 && AssetDatabase.GetAssetPath(DragAndDrop.objectReferences[0]) != ""))
            {
                insideWindow = true;
                DragAndDrop.visualMode = DragAndDropVisualMode.Link;

                if (e.rawType == EventType.DragPerform)
                {
                    foreach (Object obj in DragAndDrop.objectReferences)
                    {
                        if (AssetDatabase.GetAssetPath(obj) != "")
                        {
                            //if list is empty, add and continue
                            if(hotbarObjects.Count == 0)
                            {
                                hotbarObjects.Add(obj);
                                continue;
                            }

                            int index = widgetHost.FindClosestWidget(e.mousePosition);

                            if (index == hotbarObjects.IndexOf(obj))
                            {
                                selectedObj = obj;
                                Selection.activeObject = obj;
                                break;
                            }
                            else if (hotbarObjects.Contains(obj))
                            {
                                hotbarObjects.Remove(obj);
                                hotbarPreviewCache.Remove(obj);
                            }

                            //check if widget + margin.x contains mouse position and insert there
                            if (e.mousePosition.x > widgetHost.Widgets[index].x && e.mousePosition.x < widgetHost.Widgets[index].x + widgetHost.Widgets[index].width + widgetHost.WidgetMargin.x && e.mousePosition.y > widgetHost.Widgets[index].y && e.mousePosition.y < widgetHost.Widgets[index].y + widgetHost.Widgets[index].height)
                            {
                                //left or
                                if(e.mousePosition.x < widgetHost.Widgets[index].x + widgetHost.Widgets[index].width / 2)
                                {
                                    hotbarObjects.Insert(index, obj);
                                }
                                //right
                                else
                                {
                                    if (index < hotbarObjects.Count)
                                        hotbarObjects.Insert(index + 1, obj);
                                    else
                                        hotbarObjects.Add(obj);
                                }
                            }
                            else
                            {
                                hotbarObjects.Add(obj);
                            }
                        }
                    }
                    if(useAutoSave)
                    {
                        SaveHotbar();
                    }
                }
            }

            if (e.isKey && e.keyCode == KeyCode.Delete && selectedObj != null)
            {
                hotbarObjects.Remove(selectedObj);
                hotbarPreviewCache.Remove(selectedObj);
                if (useAutoSave)
                {
                    SaveHotbar();
                }
            }

            //reset
            if (e.rawType == EventType.MouseDown)
            {
                selectedObj = null;
            }

            widgetHost.Area = position;
            widgetHost.WidgetsCount = hotbarObjects.Count;
            widgetHost.Update();

            bool hoverSelected = false;

            float scrollviewHeight = position.height;
            if(widgetHost.Widgets.Count > 0)
            {
                if(widgetHost.Widgets[widgetHost.Widgets.Count - 1].yMax > scrollviewHeight)
                    scrollviewHeight = widgetHost.Widgets[widgetHost.Widgets.Count - 1].yMax;
            }

            GUISkin old = GUI.skin;
            GUI.skin = EditorPlus.ScrollviewSkin;
            scrollviewPosition = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), scrollviewPosition, new Rect(0, 0, position.width - 30, scrollviewHeight));
            GUI.skin = old;

            for (int i = 0; i < hotbarObjects.Count; ++i)
            {
                if (hotbarObjects[i] == null)
                {
                    hotbarObjects.RemoveAt(i);
                    --i;
                    widgetHost.WidgetsCount--;
                    continue;
                }

                GUIContent cnt = new GUIContent();
                Texture2D preview;
                hotbarPreviewCache.TryGetValue(hotbarObjects[i], out preview);
                if (preview != null)
                    cnt.image = preview;
                else
                    cnt.image = AssetDatabase.GetCachedIcon(AssetDatabase.GetAssetPath(hotbarObjects[i]));

                if(useTooltip)
                    cnt.tooltip = hotbarObjects[i].name;

                Rect r = widgetHost.Widgets[i];
                

                //object selection
                if (e.rawType == EventType.MouseDown)
                {
                    if (r.Contains(e.mousePosition))
                    {
                        //Drag object
                        if (e.button == 0)
                        {
                            if (e.clickCount != 2)
                            {
                                selectedObj = hotbarObjects[i];


                                DragAndDrop.PrepareStartDrag();
                                string path = AssetDatabase.GetAssetPath(selectedObj);
                                if (path != "")
                                    DragAndDrop.paths = new string[] { path };
                                if (selectedObj != null)
                                    DragAndDrop.objectReferences = new Object[] { selectedObj };

                                DragAndDrop.StartDrag(selectedObj.ToString());

                                e.Use();
                            }
                            else if (e.clickCount == 2)
                            {
                                if (hotbarObjects[i].GetType() == EditorPlus.EPlusDefaultType)
                                {
                                    string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                                    if (Directory.Exists(path))
                                    {
                                        string[] subFolders = Directory.GetDirectories(path);
                                        if (subFolders.Length != 0)
                                        {
                                            AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath(subFolders[0]));
                                            break;
                                        }

                                        string[] content = Directory.GetFiles(path);
                                        if(content.Length == 0)
                                        {
                                            AssetDatabase.OpenAsset(hotbarObjects[i]);
                                        }
                                        else
                                        {
                                            foreach(string file in content)
                                            {
                                                Object asset = AssetDatabase.LoadMainAssetAtPath(file);
                                                if(asset != null)
                                                {
                                                    EditorGUIUtility.PingObject(asset);
                                                    break;
                                                }
                                            }
                                            
                                        }
                                    }
                                    else
                                    {
                                        if (hotbarObjects[i].ToString().Contains("SceneAsset")) //deselect to prevent annoying merge warnings
                                            Selection.activeObject = null;
                                        AssetDatabase.OpenAsset(hotbarObjects[i]);
                                    }
                                }
                                else
                                {
                                    AssetDatabase.OpenAsset(hotbarObjects[i]);
                                }
                                    
                            }
                        }
                        else if (e.button == 1)
                        {
                            hotbarPreviewCache.Remove(hotbarObjects[i]);
                            hotbarObjects.RemoveAt(i);
                            --i;
                            GUI.EndScrollView();
                            Repaint();
                            e.Use();
                            return;
                        }
                        hoverSelected = true;
                    }
                }
                else if (e.rawType == EventType.MouseUp && r.Contains(e.mousePosition))
                    hoverSelected = true;

                GUI.Label(r, cnt, (selectedObj != hotbarObjects[i]) ? new GUIStyle(GUI.skin.button) : new GUIStyle(GUI.skin.box));

                int limit = textLimit;
                if (limit >= hotbarObjects[i].name.Length)
                    limit = hotbarObjects[i].name.Length;

                cnt = new GUIContent(hotbarObjects[i].name.Substring(0, limit));
                
                if(cnt.text.Length > textLimit)
                {
                    cnt.text.Remove(cnt.text.Length - 1, 1);
                }
                r.height = 20f;

                //drop shadow
                GUIStyle st = new GUIStyle(GUI.skin.label);
                st.normal.textColor = new Color(0.26f, 0.26f, 0.26f);
                r.x += 1f;
                r.y += 1f;
                GUI.Label(r, cnt, st);

                //reset
                r.x -= 1f;
                r.y -= 1f;

                //name
                GUI.Label(r, cnt);
            }

            GUI.EndScrollView();


            if (e.rawType == EventType.MouseUp && DragAndDrop.objectReferences != null)
            {
                DragAndDrop.PrepareStartDrag();
            }

            if (insideWindow && !hoverSelected && e.rawType == EventType.MouseUp)
            {
                selectedObj = null;
                Selection.activeObject = null;
            }
        }


        public void ClearHotbar()
        {
            hotbarObjects.Clear();
            hotbarPreviewCache.Clear();
            widgetHost.WidgetsCount = 0;
        }

        #region Save

        private void ClearAllSaves()
        {
            while (savedHotbars.Count != 0)
            {
                ClearSave(savedHotbars[0]);
            }
            EditorPrefs.SetString("EditorPlus.Hotbar.Barnames", "");
            savedHotbars.Clear();
        }


        public void ClearSave(string bar)
        {
            int count = EditorPrefs.GetInt("EditorPlus.Hotbar." + bar, -1);

            //Clear old data
            if (count != -1)
            {
                for (int i = 0; i < count; ++i)
                {
                    EditorPrefs.DeleteKey("EditorPlus.Hotbar." + bar + i.ToString());
                }
            }

            EditorPrefs.DeleteKey("EditorPlus.Hotbar." + bar);
            EditorPrefs.DeleteKey("EditorPlus.Hotbar." + bar + ".scale");
            EditorPrefs.DeleteKey("EditorPlus.Hotbar." + bar + ".limit");
            EditorPrefs.DeleteKey("EditorPlus.Hotbar." + bar + ".tooltip");

            savedHotbars.Remove(bar);
            string barNames = "";
            foreach (string barName in savedHotbars)
            {
                barNames += barName + ",";
            }
            EditorPrefs.SetString("EditorPlus.Hotbar.Barnames", barNames);
        }


        public void SaveHotbar()
        {
            int count = EditorPrefs.GetInt("EditorPlus.Hotbar." + title, -1);

            //Clear old data
            if (count != -1)
            {
                for (int i = 0; i < count; ++i)
                {
                    EditorPrefs.DeleteKey("EditorPlus.Hotbar." + title + i.ToString());
                }
            }

            count = hotbarObjects.Count;
            EditorPrefs.SetInt("EditorPlus.Hotbar." + title, count);

            for (int i = 0; i < count; ++i)
            {
                EditorPrefs.SetString("EditorPlus.Hotbar." + title + i.ToString(), AssetDatabase.GetAssetPath(hotbarObjects[i]));
            }

            EditorPrefs.SetFloat("EditorPlus.Hotbar." + title + ".scale", widgetHost.WidgetScale);
            EditorPrefs.SetInt("EditorPlus.Hotbar." + title + ".limit", textLimit);
            EditorPrefs.SetBool("EditorPlus.Hotbar." + title + ".tooltip", useTooltip);

            //save names
            if (!savedHotbars.Contains(title))
            {
                savedHotbars.Add(title);
                string barNames = "";
                foreach (string bar in savedHotbars)
                {
                    barNames += bar + ",";
                }
                EditorPrefs.SetString("EditorPlus.Hotbar.Barnames", barNames);
            }
        }


        private void LoadHotbar(object saveName)
        {
            title = (string)saveName;
            int count = EditorPrefs.GetInt("EditorPlus.Hotbar." + title, -1);
            if (count == -1)
                return;

            ClearHotbar();


            for (int i = 0; i < count; ++i)
            {
                string path = EditorPrefs.GetString("EditorPlus.Hotbar." + title + i.ToString(), "");
                if (path != "")
                    hotbarObjects.Add(AssetDatabase.LoadAssetAtPath(path, typeof(Object)));
            }

            widgetHost.WidgetScale = EditorPrefs.GetFloat("EditorPlus.Hotbar." + title + ".scale", 1f);
            textLimit = EditorPrefs.GetInt("EditorPlus.Hotbar." + title + ".limit", 3);
            useTooltip = EditorPrefs.GetBool("EditorPlus.Hotbar." + title + ".tooltip", true);
        }

        #endregion
    }
}