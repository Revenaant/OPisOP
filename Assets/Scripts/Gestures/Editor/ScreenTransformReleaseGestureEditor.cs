using TouchScript.Editor.Gestures.TransformGestures.Base;
using UnityEngine;
using UnityEditor;

namespace TouchScript.Custom
{
    [CustomEditor(typeof(ScreenTransformReleaseGesture), true)]
    internal class ScreenTransformReleaseGestureEditor : TwoPointTransformGestureBaseEditor
    {

        public static readonly GUIContent TEXT_HELP = new GUIContent("This component recognizes a gesture when all pointers are lifted off from" +
                                                                     " this GameObject BUT also recognizes a combination of translation, rotation " +
                                                                     "and scaling gestures on the GameObject in screen space.");

        public static readonly GUIContent TEXT_IGNORE_CHILDREN = new GUIContent("Ignore Children", "If selected this gesture ignores pointers from children.");
        
        private SerializedProperty ignoreChildren;
        private SerializedProperty OnRelease;

        protected override GUIContent getHelpText()
        {
            return TEXT_HELP;
        }

        protected override void OnEnable()
        {
            ignoreChildren = serializedObject.FindProperty("ignoreChildren");
            OnRelease = serializedObject.FindProperty("OnRelease");

            base.OnEnable();
        }

        protected override void drawGeneral()
        {
            EditorGUILayout.PropertyField(ignoreChildren, TEXT_IGNORE_CHILDREN);

            base.drawGeneral();
        }

        protected override void drawUnityEvents()
        {
            EditorGUILayout.PropertyField(OnRelease);

            base.drawUnityEvents();
        }
    }
}