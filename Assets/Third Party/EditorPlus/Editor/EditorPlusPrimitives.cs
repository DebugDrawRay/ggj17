#if (UNITY_4_0 || UNITY_4_0_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5)
#define EPlus_4
#else
#define EPlus_5
#endif

using UnityEngine;
using UnityEditor;
using System.Collections;

#pragma warning disable 414
#pragma warning disable 618

namespace EditorPlus
{
    public class EditorPlusPrimitives : EditorWindow
    {
        private static EditorPlusPrimitives window;

        private static Vector2 StartMargin = new Vector2(10f, 10f);

        private static Vector2 ButtonMargin = new Vector2(9f, 7f);

        private static Vector2 ButtonSize = new Vector2(100f, 20f);


        private static Texture UnityPrimitiveCubeThumb;
        private static Texture UnityPrimitiveSphereThumb;
        private static Texture UnityPrimitiveCylinderThumb;
        private static Texture UnityPrimitiveCapsuleThumb;
        private static Texture UnityPrimitivePlaneThumb;
        private static Texture UnityPrimitiveQuadThumb;

        private static Texture UnityPrimitivePointlightThumb;
        private static Texture UnityPrimitiveArealightThumb;
        private static Texture UnityPrimitiveSunlightThumb;
        private static Texture UnityPrimitiveSpotlightThumb;
        private static Texture UnityPrimitiveReflectionprobeThumb;
        private static Texture UnityPrimitiveLightprobegroupThumb;

        private static Texture UnityPrimitiveParticlesystemThumb;
        private static Texture UnityPrimitiveAudioemitterThumb;
        private static Texture UnityPrimitiveAudioreverbThumb;

        private static Texture UnityPrimitiveCameraThumb;
        private static Texture UnityPrimitiveCameraviewportThumb;
        private static Texture UnityPrimitiveTreeThumb;
        private static Texture UnityPrimitiveLandscapeThumb;
        private static Texture UnityPrimitiveSpriteThumb;
        private static Texture UnityPrimitiveNullobjectThumb;


        private static GUISkin IconsSkin;

        private static bool init = false;



        [MenuItem("Window/EditorPlus/Primitives")]
        static public void Init()
        {
            init = true;
            window = (EditorPlusPrimitives)EditorWindow.GetWindow(typeof(EditorPlusPrimitives));
            window.title = "Primitives";
            window.minSize = new Vector2(50f, 50f);
            LoadResources();

            EditorPlus.OnSkinSwitched += LoadResources;
        }


        static public void LoadResources()
        {
            UnityPrimitiveCubeThumb = (Texture)AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath +EditorPlus.SelectedSkinFolder + "UnityPlus_Icon_Cube.png");
            UnityPrimitiveSphereThumb = (Texture)AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath +EditorPlus.SelectedSkinFolder + "UnityPlus_Icon_Sphere.png");
            UnityPrimitiveCylinderThumb = (Texture)AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath +EditorPlus.SelectedSkinFolder + "UnityPlus_Icon_Cylinder.png");
            UnityPrimitiveCapsuleThumb = (Texture)AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath +EditorPlus.SelectedSkinFolder + "UnityPlus_Icon_Capsule.png");
            UnityPrimitivePlaneThumb = (Texture)AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath +EditorPlus.SelectedSkinFolder + "UnityPlus_Icon_Plane.png");
            UnityPrimitiveQuadThumb = (Texture)AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath +EditorPlus.SelectedSkinFolder + "UnityPlus_Icon_Quad.png");

            UnityPrimitivePointlightThumb = (Texture)AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath +EditorPlus.SelectedSkinFolder + "UnityPlus_Icon_Pointlight.png");
            UnityPrimitiveArealightThumb = (Texture)AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath +EditorPlus.SelectedSkinFolder + "UnityPlus_Icon_Arealight.png");
            UnityPrimitiveSunlightThumb = (Texture)AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath +EditorPlus.SelectedSkinFolder + "UnityPlus_Icon_Sunlight.png");
            UnityPrimitiveSpotlightThumb = (Texture)AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath +EditorPlus.SelectedSkinFolder + "UnityPlus_Icon_Spotlight.png");
            UnityPrimitiveReflectionprobeThumb = (Texture)AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath +EditorPlus.SelectedSkinFolder + "UnityPlus_Icon_Reflectionprobe.png");
            UnityPrimitiveLightprobegroupThumb = (Texture)AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath +EditorPlus.SelectedSkinFolder + "UnityPlus_Icon_Lightprobegroup.png");

            UnityPrimitiveParticlesystemThumb = (Texture)AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath +EditorPlus.SelectedSkinFolder + "UnityPlus_Icon_Particlesystem.png");
            UnityPrimitiveAudioemitterThumb = (Texture)AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath +EditorPlus.SelectedSkinFolder + "UnityPlus_Icon_Audioemitter.png");
            UnityPrimitiveAudioreverbThumb = (Texture)AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath +EditorPlus.SelectedSkinFolder + "UnityPlus_Icon_Audioreverb.png");

            UnityPrimitiveCameraThumb = (Texture)AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath +EditorPlus.SelectedSkinFolder + "UnityPlus_Icon_Camera.png");
            UnityPrimitiveCameraviewportThumb = (Texture)AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath +EditorPlus.SelectedSkinFolder + "UnityPlus_Icon_Cameraviewport.png");
            UnityPrimitiveTreeThumb = (Texture)AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath +EditorPlus.SelectedSkinFolder + "UnityPlus_Icon_Tree.png");
            UnityPrimitiveLandscapeThumb = (Texture)AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath +EditorPlus.SelectedSkinFolder + "UnityPlus_Icon_Landscape.png");
            UnityPrimitiveSpriteThumb = (Texture)AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath +EditorPlus.SelectedSkinFolder + "UnityPlus_Icon_Sprite.png");
            UnityPrimitiveNullobjectThumb = (Texture)AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath +EditorPlus.SelectedSkinFolder + "UnityPlus_Icon_Nullobject.png");


            ButtonSize.x = UnityPrimitiveCubeThumb.width;
            ButtonSize.y = UnityPrimitiveCubeThumb.height;
            IconsSkin = AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath +EditorPlus.SelectedSkinFolder + "UnityPlusSkinIcons.guiskin") as GUISkin;

            if(window != null)
                window.Repaint();
        }

        void Unity5()
        {
            if (GUI.skin != IconsSkin)
                GUI.skin = IconsSkin;

            Vector2 posCur = StartMargin;

            GUIContent cnt = new GUIContent(UnityPrimitiveCubeThumb);
            cnt.tooltip = "Cube";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/3D Object/Cube");
            }

            cnt = new GUIContent(UnityPrimitiveSphereThumb);
            cnt.tooltip = "Sphere";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/3D Object/Sphere");
            }

            cnt = new GUIContent(UnityPrimitiveCylinderThumb);
            cnt.tooltip = "Cylinder";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/3D Object/Cylinder");
            }

            cnt = new GUIContent(UnityPrimitiveCapsuleThumb);
            cnt.tooltip = "Capsule";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/3D Object/Capsule");
            }

            cnt = new GUIContent(UnityPrimitivePlaneThumb);
            cnt.tooltip = "Plane";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/3D Object/Plane");
            }

            cnt = new GUIContent(UnityPrimitiveQuadThumb);
            cnt.tooltip = "Quad";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/3D Object/Quad");
            }

            cnt = new GUIContent(UnityPrimitivePointlightThumb);
            cnt.tooltip = "Point Light";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Light/Point Light");
            }

            cnt = new GUIContent(UnityPrimitiveArealightThumb);
            cnt.tooltip = "Area Light";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Light/Area Light");
            }

            cnt = new GUIContent(UnityPrimitiveSunlightThumb);
            cnt.tooltip = "Directional Light";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Light/Directional Light");
            }

            cnt = new GUIContent(UnityPrimitiveSpotlightThumb);
            cnt.tooltip = "Spotlight";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Light/Spotlight");
            }

#if(!UNITY_4_6)
            cnt = new GUIContent(UnityPrimitiveLightprobegroupThumb);
            cnt.tooltip = "Light Probe Group";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Light/Light Probe Group");
            }

            cnt = new GUIContent(UnityPrimitiveReflectionprobeThumb);
            cnt.tooltip = "Reflection Probe";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Light/Reflection Probe");
            }
#endif

            cnt = new GUIContent(UnityPrimitiveParticlesystemThumb);
            cnt.tooltip = "Particle System";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Particle System");
            }

            cnt = new GUIContent(UnityPrimitiveAudioemitterThumb);
            cnt.tooltip = "Audio Source";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Audio/Audio Source");
            }

            cnt = new GUIContent(UnityPrimitiveAudioreverbThumb);
            cnt.tooltip = "Audio Reverb Zone";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Audio/Audio Reverb Zone");
            }


            cnt = new GUIContent(UnityPrimitiveCameraviewportThumb);
            cnt.tooltip = "Camera Viewport aligned";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Camera");
                GameObject goCam = Selection.activeGameObject;
                if (goCam != null && SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null)
                {
                    goCam.transform.position = SceneView.lastActiveSceneView.camera.transform.position;
                    goCam.transform.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;
                }
            }

            cnt = new GUIContent(UnityPrimitiveCameraThumb);
            cnt.tooltip = "Camera";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Camera");
            }

            cnt = new GUIContent(UnityPrimitiveTreeThumb);
            cnt.tooltip = "Tree";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/3D Object/Tree");
            }

            cnt = new GUIContent(UnityPrimitiveLandscapeThumb);
            cnt.tooltip = "Terrain";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/3D Object/Terrain");
            }

            cnt = new GUIContent(UnityPrimitiveSpriteThumb);
            cnt.tooltip = "Sprite";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/2D Object/Sprite");
            }

            cnt = new GUIContent(UnityPrimitiveNullobjectThumb);
            cnt.tooltip = "Create Empty";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Create Empty");
            }
        }

        void Unity4()
        {
            if (GUI.skin != IconsSkin)
                GUI.skin = IconsSkin;

            Vector2 posCur = StartMargin;

            GUIContent cnt = new GUIContent(UnityPrimitiveCubeThumb);
            cnt.tooltip = "Cube";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Create Other/Cube");
            }

            cnt = new GUIContent(UnityPrimitiveSphereThumb);
            cnt.tooltip = "Sphere";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Create Other/Sphere");
            }

            cnt = new GUIContent(UnityPrimitiveCylinderThumb);
            cnt.tooltip = "Cylinder";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Create Other/Cylinder");
            }

            cnt = new GUIContent(UnityPrimitiveCapsuleThumb);
            cnt.tooltip = "Capsule";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Create Other/Capsule");
            }

            cnt = new GUIContent(UnityPrimitivePlaneThumb);
            cnt.tooltip = "Plane";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Create Other/Plane");
            }

            cnt = new GUIContent(UnityPrimitiveQuadThumb);
            cnt.tooltip = "Quad";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Create Other/Quad");
            }


            cnt = new GUIContent(UnityPrimitivePointlightThumb);
            cnt.tooltip = "Point Light";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Create Other/Point Light");
            }

            cnt = new GUIContent(UnityPrimitiveArealightThumb);
            cnt.tooltip = "Area Light";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Create Other/Area Light");
            }

            cnt = new GUIContent(UnityPrimitiveSunlightThumb);
            cnt.tooltip = "Directional Light";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Create Other/Directional Light");
            }

            cnt = new GUIContent(UnityPrimitiveSpotlightThumb);
            cnt.tooltip = "Spotlight";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Create Other/Spotlight");
            }


            cnt = new GUIContent(UnityPrimitiveParticlesystemThumb);
            cnt.tooltip = "Particle System";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Create Other/Particle System");
            }

            cnt = new GUIContent(UnityPrimitiveAudioreverbThumb);
            cnt.tooltip = "Audio Reverb Zone";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Create Other/Audio Reverb Zone");
            }


            cnt = new GUIContent(UnityPrimitiveCameraviewportThumb);
            cnt.tooltip = "Camera Viewport aligned";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Create Other/Camera");
                GameObject goCam = Selection.activeGameObject;
                if (goCam != null && SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null)
                {
                    goCam.transform.position = SceneView.lastActiveSceneView.camera.transform.position;
                    goCam.transform.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;
                }
            }

            cnt = new GUIContent(UnityPrimitiveCameraThumb);
            cnt.tooltip = "Camera";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Create Other/Camera");
            }

            cnt = new GUIContent(UnityPrimitiveTreeThumb);
            cnt.tooltip = "Tree";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Create Other/Tree");
            }

            cnt = new GUIContent(UnityPrimitiveLandscapeThumb);
            cnt.tooltip = "Terrain";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Create Other/Terrain");
            }

            cnt = new GUIContent(UnityPrimitiveNullobjectThumb);
            cnt.tooltip = "Create Empty";
            if (GUI.Button(EditorPlus.GetButtonRectByPos(ref posCur, ButtonSize, ButtonMargin, StartMargin, window.position), cnt))
            {
                EditorApplication.ExecuteMenuItem("GameObject/Create Empty");
            }
        }

        void OnGUI()
        {
            if (!init)
                Init();

#if EPlus_5
            Unity5();
#elif EPlus_4
            Unity4();
#endif
        }
    }
}