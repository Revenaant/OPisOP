// *******************************************************
// Copyright 2013 Daikon Forge, all rights reserved under 
// US Copyright Law and international treaties
// *******************************************************

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

#pragma warning disable 0618
//[InitializeOnLoad]
//public class AutosaveOnRun : ScriptableObject
//{
//    static AutosaveOnRun()
//    {
//        EditorApplication.playmodeStateChanged = () =>
//        {
//            if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
//            {
//                Debug.Log("Auto-Saving scene before entering Play mode: " + EditorApplication.currentScene);

//                EditorApplication.SaveScene();
//                AssetDatabase.SaveAssets();
//            }
//        };
//    }
//}
#pragma warning restore 0618

//using UnityEngine;
//using UnityEditor;
//using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class OnUnityLoad
{

    static OnUnityLoad()
    {

        EditorApplication.playModeStateChanged += (PlayModeStateChange mode) =>
        {

            //if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
            if (mode == PlayModeStateChange.ExitingEditMode)
            {
                var currentScene = EditorSceneManager.GetActiveScene();
                Debug.Log("Auto-Saving scene before entering Play mode: " + currentScene);

                EditorSceneManager.SaveScene(currentScene);
                AssetDatabase.SaveAssets();
            }
        };

    }
}
#endif
