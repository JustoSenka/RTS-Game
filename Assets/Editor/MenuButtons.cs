using UnityEngine;
using UnityEditor;
using System.Collections;

public class MenuButtons : Editor {

	[MenuItem("AssetBundle/Build Asset Bundles")]
	public static void BuildAssetBundles()
	{
		BuildPipeline.BuildAssetBundles("Assets/Asset Bundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
	}
}
