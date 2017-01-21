#if (UNITY_4_2 || UNITY_4_3 || UNITY_4_5)
#define EPlus_4
#else
#define EPlus_5
#endif

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable 618

namespace EditorPlus
{
    public class EditorPlusShortcuts : EditorWindow, IHasCustomMenu
    {
        private static EditorPlusShortcuts window;

        private static Vector2 StartMargin = new Vector2(6, 6f);

        private static Vector2 ButtonMargin = new Vector2(6f, 4f);

        private static Vector2 ButtonSize = new Vector2(120f, 20f);

        private static bool init = false;

        private static GUISkin ShortcutsSkin;

        private static GUISkin ShortcutsSkinEdit;

        private static List<string> Shortcuts;

        private static List<string> ShortcutsDefaults;

        private static List<string> ShortcutsDefaultsUI;

        private static List<string> ShortcutsCustoms;

        private static bool enableEditing = false;

        private static string editingNewPath = "";

        private static Vector2 scrollViewPos = Vector2.zero;

        private static WidgetHost widgetHost;

        private static Rect editWindowRect = new Rect(100, 100, 400, 300);

        private static Vector2 editWindowViewpos = Vector2.zero;


        [MenuItem("Window/EditorPlus/Shortcuts")]
        static public void Init()
        {
            init = true;
            window = (EditorPlusShortcuts)EditorWindow.GetWindow(typeof(EditorPlusShortcuts));
            window.title = "Shortcuts";
            window.minSize = new Vector2(StartMargin.x + ButtonSize.x + ButtonMargin.x, StartMargin.y + ButtonSize.y + ButtonMargin.y);

            widgetHost = new WidgetHost(window.position, StartMargin, ButtonMargin, ButtonSize, 0, 1f, true);

            ShortcutsSkinEdit = AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath + "UnityPlusSkinShortcutsEdit.guiskin") as GUISkin;

            if (Shortcuts == null)
                Shortcuts = new List<string>();

            if (ShortcutsDefaults == null)
                ShortcutsDefaults = new List<string>();

            if (ShortcutsCustoms == null)
                ShortcutsCustoms = new List<string>();

            SaveDefaults();

#if EPlus_5
            SaveDefaultsUI();
#endif

            LoadAllSaves();
            LoadResources();

            EditorPlus.OnSkinSwitched += LoadResources;
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

                foreach (string def in ShortcutsDefaults)
                {
                    string[] name = def.Split('/');
                    if (!Shortcuts.Contains(def))
                    {
                        menu.AddItem(new GUIContent("Add Item/Defaults/" + name[name.Length - 1]), false, AddToActive, def);
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent("Add Item/Defaults/" + name[name.Length - 1]));
                    }
                }

                if (ShortcutsDefaults.Count > 0)
                {
                    menu.AddSeparator("Add Item/Defaults/");
                    menu.AddItem(new GUIContent("Add Item/Defaults/Add All"), false, AddAllDefaults);
                }

                foreach (string def in ShortcutsDefaultsUI)
                {
                    string[] name = def.Split('/');
                    if (!Shortcuts.Contains(def))
                    {
                        menu.AddItem(new GUIContent("Add Item/DefaultsUI/" + name[name.Length - 1]), false, AddToActive, def);
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent("Add Item/DefaultsUI/" + name[name.Length - 1]));
                    }
                }

                if (ShortcutsDefaults.Count > 0)
                {
                    menu.AddSeparator("Add Item/DefaultsUI/");
                    menu.AddItem(new GUIContent("Add Item/DefaultsUI/Add All"), false, AddAllDefaultsUI);
                }

                foreach (string cust in ShortcutsCustoms)
                {
                    string[] name = cust.Split('/');
                    if (!Shortcuts.Contains(cust))
                    {
                        menu.AddItem(new GUIContent("Add Item/Customs/" + name[name.Length - 1]), false, AddToActive, cust);
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent("Add Item/Customs/" + name[name.Length - 1]));
                    }
                }

                if (ShortcutsCustoms.Count > 0)
                {
                    menu.AddSeparator("Add Item/Customs/");
                    menu.AddItem(new GUIContent("Add Item/Customs/Add All"), false, AddAllCustoms);
                }

                for (int i = 0; i < Shortcuts.Count; ++i)
                {
                    string[] name = Shortcuts[i].Split('/');
                    int ind = i;
                    menu.AddItem(new GUIContent("Remove Item/" + name[name.Length - 1]), false, RemoveFromActive, (object)Shortcuts[i]);
                    if (ind != i)
                        i = ind;    //necessary since we delete items during loop
                }

                if (Shortcuts.Count > 0)
                {
                    menu.AddSeparator("Remove Item/");
                    menu.AddItem(new GUIContent("Remove Item/Remove All"), false, ClearActive);
                }

                menu.AddSeparator("");

                menu.AddItem(new GUIContent("Add Custom Button"), false, EnableEditing);


                menu.ShowAsContext();
            }
        }


        public static void SaveDefaults()
        {
            if(EditorPrefs.GetString("EditorPlusShortcuts.Defaults", "") != "")
                return;
            List<string> Defaults = new List<string>();
            Defaults.Add("Edit/Project Settings/Player");
            Defaults.Add("Edit/Project Settings/Quality");
            Defaults.Add("Edit/Project Settings/Input");
            Defaults.Add("Edit/Project Settings/Physics");
            Defaults.Add("Window/Asset Store");
#if EPlus_5
            Defaults.Add("Edit/Project Settings/Physics 2D");
#if(!UNITY_4_6)
            Defaults.Add("Window/Occlusion Culling");
            Defaults.Add("Window/Lighting");
#endif
            Defaults.Add("Window/Profiler");
#endif

            string result = "";
            foreach(string entry in Defaults)
            {
                result += entry + ",";
            }
            if(result.Length == 0)
                return;
            result.Remove(result.Length - 1);

            EditorPrefs.SetString("EditorPlusShortcuts.Defaults", result);

            Shortcuts.AddRange(Defaults);
            SaveActives();
        }


        public static void SaveDefaultsUI()
        {
            if (EditorPrefs.GetString("EditorPlusShortcuts.DefaultsUI", "") != "")
                return;
            List<string> DefaultsUI = new List<string>();
            DefaultsUI.Add("GameObject/UI/Panel");
            DefaultsUI.Add("GameObject/UI/Button");
            DefaultsUI.Add("GameObject/UI/Text");
            DefaultsUI.Add("GameObject/UI/Image");
            DefaultsUI.Add("GameObject/UI/Raw Image");
            DefaultsUI.Add("GameObject/UI/Slider");
            DefaultsUI.Add("GameObject/UI/Scrollbar");
            DefaultsUI.Add("GameObject/UI/Toggle");
            DefaultsUI.Add("GameObject/UI/Input Field");
            DefaultsUI.Add("GameObject/UI/Canvas");
            DefaultsUI.Add("GameObject/UI/Event System");
            DefaultsUI.Add("GameObject/2D Object/Sprite");

            string result = "";
            foreach (string entry in DefaultsUI)
            {
                result += entry + ",";
            }
            if (result.Length == 0)
                return;
            result.Remove(result.Length - 1);

            EditorPrefs.SetString("EditorPlusShortcuts.DefaultsUI", result);

        }


        public static void SaveCustoms()
        {
            string result = "";
            foreach(string custom in ShortcutsCustoms)
            {
                result += custom + ",";
            }
            if(result.Length == 0)
                return;
            result.Remove(result.Length - 1);

            EditorPrefs.SetString("EditorPlusShortcuts.Customs", result);
        }


        public static void SaveActives()
        {
            string result = "";
            foreach (string act in Shortcuts)
            {
                result += act + ",";
            }
            if (result.Length == 0)
                return;
            result.Remove(result.Length - 1);

            EditorPrefs.SetString("EditorPlusShortcuts.Actives", result);
        }


        static public void LoadAllSaves()
        {
            ShortcutsDefaults = new List<string>();
            ShortcutsDefaultsUI = new List<string>();
            ShortcutsCustoms = new List<string>();
            Shortcuts = new List<string>();

            string[] savedDefaults = EditorPrefs.GetString("EditorPlusShortcuts.Defaults", "").Split(',');
            foreach (string def in savedDefaults)
            {
                ShortcutsDefaults.Add(def);
            }

            string[] savedDefaultsUI = EditorPrefs.GetString("EditorPlusShortcuts.DefaultsUI", "").Split(',');
            foreach (string def in savedDefaultsUI)
            {
                ShortcutsDefaultsUI.Add(def);
            }
            
            string[] savedCustoms = EditorPrefs.GetString("EditorPlusShortcuts.Customs", "").Split(',');
            foreach(string custom in savedCustoms)
            {
                if (custom == "")
                    continue;
                ShortcutsCustoms.Add(custom);
            }

            string[] savedActives = EditorPrefs.GetString("EditorPlusShortcuts.Actives", "").Split(',');
            foreach (string act in savedActives)
            {
                Shortcuts.Add(act);
            }
        }


        static public void AddToActive(object act)
        {
            if (!Shortcuts.Contains((string)act))
            {
                Shortcuts.Add((string)act);
                SaveActives();
            }
        }


        static public void AddToCustoms(string cust)
        {
            if (!ShortcutsCustoms.Contains(cust))
            {
                ShortcutsCustoms.Add(cust);
                SaveCustoms();
            }
        }


        static public void DeleteCustom(string cust)
        {
            if (ShortcutsCustoms.Contains(cust))
            {
                ShortcutsCustoms.Remove(cust);
                if(Shortcuts.Contains(cust))
                {
                    Shortcuts.Remove(cust);
                }
                SaveCustoms();
            }
        }


        static public void AddAllDefaults()
        {
            foreach(string def in ShortcutsDefaults)
            {
                AddToActive(def);
            }
        }


        static public void AddAllDefaultsUI()
        {
            foreach (string def in ShortcutsDefaultsUI)
            {
                AddToActive(def);
            }
        }


        static public void AddAllCustoms()
        {
            foreach(string cust in ShortcutsCustoms)
            {
                AddToActive(cust);
            }
        }


        static public void RemoveFromActive(object act)
        {
            if (Shortcuts.Contains((string)act))
            {
                Shortcuts.Remove((string)act);
                SaveActives();
            }
        }


        static public void ClearActive()
        {
            Shortcuts.Clear();
            SaveActives();
        }

        
        static public void EnableEditing()
        {
            enableEditing = true;
        }


        static public void LoadResources()
        {
            ShortcutsSkin = AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath + EditorPlus.SelectedSkinFolder + "UnityPlusSkinShortcuts.guiskin") as GUISkin;
            
            window.Repaint();
        }


        void OnGUI()
        {
            if (!init)
                Init();

            if (GUI.skin != ShortcutsSkin)
                GUI.skin = ShortcutsSkin;

            widgetHost.Area = position;
            widgetHost.WidgetsCount = Shortcuts.Count;
            widgetHost.Update();

            float scrollviewHeight = 0f;
            if (widgetHost.Widgets.Count > 0)
            {
                scrollviewHeight = widgetHost.Widgets[widgetHost.Widgets.Count - 1].yMax;
            }

            GUISkin old = GUI.skin;
            GUI.skin = EditorPlus.ScrollviewSkin;
            scrollViewPos = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), scrollViewPos, new Rect(0, 0, position.width - 30, scrollviewHeight));
            GUI.skin = old;

            for (int i = 0; i < Shortcuts.Count; ++i )
            {
                if (Shortcuts[i] == "")
                {
                    Shortcuts.RemoveAt(i);
                    --i;
                    continue;
                }
                string[] splitted = Shortcuts[i].Split('/');

                if (ShortcutsCustoms.Contains(Shortcuts[i]))
                {
                    splitted[splitted.Length - 1] = "• " + splitted[splitted.Length - 1];
                }

                if (GUI.Button(widgetHost.Widgets[i], splitted[splitted.Length - 1]))
                {
                    if (!EditorApplication.ExecuteMenuItem(Shortcuts[i]))
                    {
                        Debug.LogWarning("Executing Menu Item failed, check if path is valid: " + Shortcuts[i]);
                    }
                }
            }

            GUI.EndScrollView();

            if(enableEditing)
            {
                GUI.skin = ShortcutsSkinEdit;
                BeginWindows();
                editWindowRect = GUI.Window(1, editWindowRect, WindowGUI, "Add Custom Button");
                EndWindows();
                GUI.skin = ShortcutsSkin;
            }
        }


        void WindowGUI(int windID)
        {
            editWindowViewpos = GUILayout.BeginScrollView(editWindowViewpos);


            GUILayout.Label("Click Paths below to delete entry");
            for (int i = 0; i < ShortcutsCustoms.Count; ++i )
            {
                if (GUILayout.Button(ShortcutsCustoms[i]))
                {
                    DeleteCustom(ShortcutsCustoms[i]);
                    --i;
                }
            }

            GUILayout.Label("Add new Path here");

            editingNewPath = GUILayout.TextField(editingNewPath);

            GUILayout.Label("");

            GUILayout.Label("Example Path: Window/EditorPlus/EditorPlusShortcuts");

            GUILayout.Label("Unity prevents opening certain Paths (File/NewScene and others)");

            bool returned = false;
            if(Event.current.keyCode == KeyCode.Return)
            {
                returned = true;
                Event.current.Use();
            }

            if ((GUILayout.Button("Add Button from Path") || returned) && editingNewPath.Length != 0)
            {
                AddToCustoms(editingNewPath);
                AddToActive(editingNewPath);
                editingNewPath = "";
                Repaint();
            }

            if(GUILayout.Button("Close Window"))
            {
                editingNewPath = "";
                enableEditing = false;
            }

            GUILayout.EndScrollView();

            GUI.DragWindow();
        }
    }
}