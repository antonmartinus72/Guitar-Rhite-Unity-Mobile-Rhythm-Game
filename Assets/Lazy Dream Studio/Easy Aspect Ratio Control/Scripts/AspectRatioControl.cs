using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EasyAspectRatio 
{
    [AddComponentMenu("Easy Aspect Ratio/Aspect Ratio Control")]
    public class AspectRatioControl : MonoBehaviour
    {
        public Vector2 targetAspectRatio = new Vector2(16.0f, 9.0f);
        private Camera blackBarCamera;

        void Start()
        {
            // set the desired aspect ratio
            float targetratio = targetAspectRatio.x / targetAspectRatio.y;

            // get current game window aspect ratio
            float windowratio = (float)Screen.width / (float)Screen.height;

            // viewport height multiplier
            float scaleheight = windowratio / targetratio;

            // get the camera component
            Camera camera = GetComponent<Camera>();

            // if scaled height is less than current height, add letterbox
            if (scaleheight < 1.0f)
            {
                Rect rect = camera.rect;

                rect.width = 1.0f;
                rect.height = scaleheight;
                rect.x = 0;
                rect.y = (1.0f - scaleheight) / 2.0f;

                camera.rect = rect;
            }
            else // add pillarbox
            {
                float scalewidth = 1.0f / scaleheight;

                Rect rect = camera.rect;

                rect.width = scalewidth;
                rect.height = 1.0f;
                rect.x = (1.0f - scalewidth) / 2.0f;
                rect.y = 0;

                camera.rect = rect;
            }

            // create a new gameobject and add a Camera component to it
            blackBarCamera = new GameObject("BlackBar Camera").AddComponent<Camera>();

            // set the parameters of the 'BlackBarCamera' to render the black bar region (letterbox and pillarbox)
            blackBarCamera.clearFlags = CameraClearFlags.SolidColor;
            blackBarCamera.backgroundColor = Color.black;
            blackBarCamera.depth = Camera.main.depth - 1;

            // set the 'BlackBarCamer'a as a child of the main camera
            blackBarCamera.transform.SetParent(Camera.main.transform);
            blackBarCamera.transform.position = Vector3.zero;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AspectRatioControl))]
    public class AspectRatioControlEditor : Editor
    {
        private const string componentDescription = "Make sure this component is attached on the Main Camera";

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(componentDescription, MessageType.Info);
            DrawDefaultInspector();
        }
    }
#endif
}
