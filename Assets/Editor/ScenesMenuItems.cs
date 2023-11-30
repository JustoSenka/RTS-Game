using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesMenuItems
{
    public const string k_SetupAssetPath = "Assets/Editor/SceneSetup.asset";

    [MenuItem("Tools/Play Menu Scene")]
    public static void LoadMenuScene()
    {
        var isMenuSceneLoaded = GetAllScenes().Count() == 1 && GetAllScenes().First().buildIndex == 0;

        if (!isMenuSceneLoaded)
        {
            var anyDirtyScenesWeNeedToSave = GetAllScenes().Any(s => s.isDirty);
            if (anyDirtyScenesWeNeedToSave)
            {
                var didSave = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                if (!didSave)
                    return;
            }

            EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(0), OpenSceneMode.Single);
        }

        EditorApplication.isPlaying = true;
    }

    [MenuItem("Tools/Scenes/Save Scene Manager Setup")]
    public static void SavesSetup()
    {
        var asset = ScriptableObject.CreateInstance<SceneManagerSetupAsset>();
        asset.SceneSetup = EditorSceneManager.GetSceneManagerSetup();
        AssetDatabase.CreateAsset(asset, k_SetupAssetPath);
    }

    [MenuItem("Tools/Scenes/Restore Scene Manager Setup")]
    public static void RestoreSetup()
    {
        var asset = AssetDatabase.LoadAssetAtPath<SceneManagerSetupAsset>(k_SetupAssetPath);
        EditorSceneManager.RestoreSceneManagerSetup(asset.SceneSetup);
    }

    private static IEnumerable<Scene> GetAllScenes()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
            yield return SceneManager.GetSceneAt(i);
    }
}
