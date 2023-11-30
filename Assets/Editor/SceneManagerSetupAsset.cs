using System;
using UnityEditor.SceneManagement;
using UnityEngine;

[Serializable]
public class SceneManagerSetupAsset : ScriptableObject
{
    public SceneSetup[] SceneSetup;
}
