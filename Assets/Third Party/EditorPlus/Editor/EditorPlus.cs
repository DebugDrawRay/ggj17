#if (UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6)
#define EPlus_4
#else
#define EPlus_5
#endif

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;


namespace EditorPlus
{
    public static class EditorPlus
    {

        public static string[] skinfolder = { "Skin_1/", "Skin_2/", "Skin_3/", "Skin_4/", "Skin_5/" };

        public static Texture EditorPlusIcon;

        public static GUISkin ScrollviewSkin;

        public static int UnityTextureCache;

        private static int selectedId = 0;

        public static Type EPlusDefaultType;

        public static int SelectedSkinId
        {
            set
            {
                SwitchSkin(value);
            }
            get
            {
                return selectedId;
            }
        }


        public static string SelectedSkinFolder = skinfolder[0];


        public delegate void ReloadResources();


        public static ReloadResources OnSkinSwitched;

        public static string PlusPath = "Assets/EditorPlus/Content/";



        static EditorPlus()
        {
            PlusPath = "";
            FindEPlusDir("Assets");
#if EPlus_4
            EPlusDefaultType = typeof(UnityEngine.Object);
#else
            EPlusDefaultType = typeof(UnityEditor.DefaultAsset);
#endif

            SwitchSkin(EditorPrefs.GetInt("EditorPlus.SelectedSkin", 0));
            EditorPlusIcon = AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath + "EditorPlus_PlusIcon.png") as Texture;
            ScrollviewSkin = AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath + "UnityPlusSkinScrollbar.guiskin") as GUISkin;
            UnityTextureCache = EditorPrefs.GetInt("EditorPlus.UnityCacheSize", 50);
            SetUnityCacheSize(UnityTextureCache);
        }

        
        public static void FindEPlusDir(string dir)
        {
            if (PlusPath.Contains("EditorPlus"))
                return;
            try
            {
                foreach (string d in Directory.GetDirectories(dir))
                {
                    if(d.Contains("EditorPlus"))
                    {
                        d.Remove(d.Length - 1, 1);
                        PlusPath = d + "/Content/";
                        return;
                    }
                    FindEPlusDir(d);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }


        public static bool SwitchSkin(int skinId)
        {
            if (skinId < 0 || skinId >= skinfolder.Length || skinId == selectedId)
            {
                return false;
            }

            selectedId = skinId;
            SelectedSkinFolder = skinfolder[selectedId];

            EditorPrefs.SetInt("EditorPlus.SelectedSkin", skinId);

            if (OnSkinSwitched != null)
            {
                OnSkinSwitched();
            }

            return true;
        }

        static public void Skin1()
        {
            SwitchSkin(0);
        }

        static public void Skin2()
        {
            SwitchSkin(1);
        }

        static public void Skin3()
        {
            SwitchSkin(2);
        }

        static public void Skin4()
        {
            SwitchSkin(3);
        }

        static public void Skin5()
        {
            SwitchSkin(4);
        }



        public static void SetUnityCacheSize(int size)
        {
            UnityTextureCache = size;
#if EPlus_5
            EditorPrefs.SetInt("EditorPlus.UnityCacheSize", size);
            AssetPreview.SetPreviewTextureCacheSize(size);
#endif
        }




        public static Rect GetScaledButtonRectByPos(ref Vector2 posCur, Vector2 buttonSize, Vector2 buttonMargin, Vector2 startMargin, Rect area)
        {
            float widthMinMarg = area.width - startMargin.x;
            float buttonSizeWithMarg = buttonSize.x + buttonMargin.x;
            float buttonsPerRow = Mathf.Floor(widthMinMarg / buttonSizeWithMarg);
            float modWidth = widthMinMarg % buttonSizeWithMarg;
            float newWidth = Mathf.Floor(buttonSize.x + modWidth / buttonsPerRow);


            if (buttonsPerRow > 0)
                buttonSize.x = newWidth;

            Rect res = new Rect(posCur.x, posCur.y, buttonSize.x, buttonSize.y);

            posCur.x += buttonSize.x + buttonMargin.x;

            if (posCur.x + buttonSize.x + buttonMargin.x > area.width)
            {
                posCur.x = startMargin.x;
                posCur.y += buttonSize.y + buttonMargin.y;
            }

            return res;
        }


        public static Rect GetScaledButtonRectByPos(ref Vector2 posCur, Vector2 buttonSize, Vector2 buttonMargin, Vector2 startMargin, EditorWindow window, Vector2 lockButtonSize, ref Rect lockButton)
        {
            float widthMinMarg = window.position.width - startMargin.x;
            float buttonSizeWithMarg = buttonSize.x + lockButtonSize.x + buttonMargin.x;
            float buttonsPerRow = Mathf.Floor(widthMinMarg / buttonSizeWithMarg);
            float modWidth = widthMinMarg % buttonSizeWithMarg;
            float newWidth = Mathf.Floor(buttonSize.x + modWidth / buttonsPerRow);


            if (buttonsPerRow > 0)
                buttonSize.x = newWidth;

            Rect res = new Rect(posCur.x, posCur.y, buttonSize.x, buttonSize.y);

            posCur.x += buttonSize.x;

            lockButton.x = posCur.x;
            lockButton.y = posCur.y;
            lockButton.width = lockButtonSize.x;
            lockButton.height = lockButtonSize.y;

            posCur.x += lockButton.width + buttonMargin.x;
            if (posCur.x + buttonSize.x + buttonMargin.x > window.position.width)
            {
                posCur.x = startMargin.x;
                posCur.y += buttonSize.y + buttonMargin.y;
            }

            return res;
        }


        public static Rect GetButtonRectByPos(ref Vector2 posCur, Vector2 buttonSize, Vector2 buttonMargin, Vector2 startMargin, Rect area)
        {
            Rect res = new Rect(posCur.x, posCur.y, buttonSize.x, buttonSize.y);

            posCur.x += buttonSize.x + buttonMargin.x;
            if (posCur.x + buttonSize.x + buttonMargin.x > area.width)
            {
                posCur.x = startMargin.x;
                posCur.y += buttonSize.y + buttonMargin.y;
            }

            return res;
        }


        public static Rect GetButtonRectByPos(ref Vector2 posCur, Vector2 buttonSize, Vector2 buttonMargin, Vector2 startMargin, EditorWindow window, Vector2 lockButtonSize, ref Rect lockButton)
        {
            Rect res = new Rect(posCur.x, posCur.y, buttonSize.x, buttonSize.y);

            posCur.x += buttonSize.x;

            lockButton.x = posCur.x;
            lockButton.y = posCur.y;
            lockButton.width = lockButtonSize.x;
            lockButton.height = lockButtonSize.y;

            posCur.x += lockButton.width + buttonMargin.x;
            if (posCur.x + buttonSize.x + buttonMargin.x > window.position.width)
            {
                posCur.x = startMargin.x;
                posCur.y += buttonSize.y + buttonMargin.y;
            }

            return res;
        }
    }

    [Serializable]
    public class WidgetHost
    {
        [SerializeField]
        private Rect area;

        [SerializeField]
        private Vector2 startMargin;

        [SerializeField]
        private Vector2 widgetMargin;

        [SerializeField]
        private Vector2 widgetSize;

        [SerializeField]
        private float widgetScale;

        [SerializeField]
        private int widgetsCount;

        [SerializeField]
        private List<Rect> widgets;

        [SerializeField]
        private bool scale;

        [SerializeField]
        private bool dirty;


        /// <summary>
        /// The area to host the widgets
        /// </summary>
        public Rect Area
        {
            get
            {
                return area;
            }
            set
            {
                if( area != value)
                {
                    area = value;
                    dirty = true;
                }
            }
        }

        public Vector2 StartMargin
        {
            get
            {
                return startMargin;
            }
            set
            {
                if (startMargin != value)
                {
                    startMargin = value;
                    dirty = true;
                }
            }
        }

        public Vector2 WidgetMargin
        {
            get
            {
                return widgetMargin;
            }
            set
            {
                if (widgetMargin != value)
                {
                    widgetMargin = value;
                    dirty = true;
                }
            }
        }

        public Vector2 WidgetSize
        {
            get
            {
                return widgetSize;
            }
            set
            {
                if (widgetSize != value)
                {
                    widgetSize = value;
                    dirty = true;
                }
            }
        }

        public float WidgetScale
        {
            get
            {
                return widgetScale;
            }
            set
            {
                if (widgetScale != value)
                {
                    widgetScale = value;
                    dirty = true;
                }
            }
        }

        public int WidgetsCount
        {
            get
            {
                return widgetsCount;
            }
            set
            {
                if (widgetsCount != value)
                {
                    widgetsCount = value;
                    dirty = true;
                }
            }
        }

        public List<Rect> Widgets
        {
            get
            {
                return widgets;
            }
        }

        public bool Scale
        {
            get
            {
                return scale;
            }
            set
            {
                if (scale != value)
                {
                    scale = value;
                    dirty = true;
                }
            }
        }


        public WidgetHost(Rect area, Vector2 startMargin, Vector2 widgetMargin, Vector2 widgetSize, int widgetsCount, float widgetsScale,bool autoScale)
        {
            this.area = area;
            this.startMargin = startMargin;
            this.widgetMargin = widgetMargin;
            this.widgetSize = widgetSize;
            this.widgetsCount = widgetsCount;
            this.widgetScale = widgetsScale;
            this.scale = autoScale;
            this.widgets = new List<Rect>();
            this.dirty = true;
        }


        public void Update()
        {
            if (!dirty)
                return;

            widgets.Clear();

            if (scale)
                UpdateScaled();
            else
                UpdateNonScaled();
            dirty = false;
        }


        private void UpdateNonScaled()
        {
            Vector2 posCur = startMargin; 
            while (widgets.Count < widgetsCount)
            {
                widgets.Add(EditorPlus.GetButtonRectByPos(ref posCur, widgetSize * widgetScale, widgetMargin, startMargin, area));
            }
        }


        private void UpdateScaled()
        {
            Vector2 posCur = startMargin;
            while (widgets.Count < widgetsCount)
            {
                widgets.Add(EditorPlus.GetScaledButtonRectByPos(ref posCur, widgetSize * widgetScale, widgetMargin, startMargin, area));
            }
        }


        public int FindClosestWidget(Vector2 position)
        {
            int index = -1;
            float curDistance = float.MaxValue;

            for(int i = 0; i < widgets.Count; ++i)
            {
                float dist = Vector2.Distance(position, new Vector2(widgets[i].x, widgets[i].y) + widgetSize / 2);
                if (dist < curDistance)
                {
                    index = i;
                    curDistance = dist;
                }
            }
            return index;
        }


        public float GetYMax()
        {
            if (widgets.Count == 0)
                return 0f;
            return widgets[widgets.Count - 1].yMax;
        }
    }
}