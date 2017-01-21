#if (UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6)
#define EPlus_4
#else
#define EPlus_5
#endif

using UnityEngine;
using UnityEditor;
using System.Collections;


#pragma warning disable 618

namespace EditorPlus
{
    public class EditorPlusConfig : EditorWindow
    {
        private static EditorPlusConfig window;

        private bool init = false;

        private Vector2 ScrollViewPosition = Vector2.zero;

        



        [MenuItem("Window/EditorPlus/Config")]
        static public void Init()
        {
            window = (EditorPlusConfig)EditorWindow.GetWindow(typeof(EditorPlusConfig));
            window.title = "Config";
            window.minSize = new Vector2(200, 300);
        }

        void OnEnable()
        {
            init = true;
        }

        void OnGUI()
        {
            if (!init)
                return;

            EditorGUILayout.BeginScrollView(ScrollViewPosition);

            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("Editor Plus Config");

#if EPlus_5
            int cacheSize = EditorPlus.UnityTextureCache;

            cacheSize = EditorGUILayout.IntSlider("Unity Preview Cache Size", cacheSize, 20, 500);
            if (EditorPlus.UnityTextureCache != cacheSize)
                EditorPlus.SetUnityCacheSize(cacheSize);

#endif

            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("Skins");
            if (GUILayout.Button("Light 1"))
            {
                EditorPlus.Skin1();
            }
            if (GUILayout.Button("Light 2"))
            {
                EditorPlus.Skin2();
            }
            if (GUILayout.Button("Light 3"))
            {
                EditorPlus.Skin3();
            }
            if (GUILayout.Button("Dark 1"))
            {
                EditorPlus.Skin4();
            }
            if (GUILayout.Button("Dark 2"))
            {
                EditorPlus.Skin5();
            }

            EditorGUILayout.LabelField("");

            EditorPlusHistoryInternal.historyUseSceneSaving = GUILayout.Toggle(EditorPlusHistoryInternal.historyUseSceneSaving, "Save History Objects in Scenes");

            if(GUILayout.Button("Remove save data in scene files"))
            {
                EditorPlusClearScenes.Run();
            }

            if (GUILayout.Button("Readme"))
            {
                AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath + "EditorPlus_Readme.pdf"));
            }

            if (GUILayout.Button("Website"))
            {
                Application.OpenURL("http://www.flowfiregames.com/");
            }


            GUILayout.EndScrollView();
        }
    }
}