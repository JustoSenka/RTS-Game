using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Data : MonoBehaviour
{
	void Awake() { Instance = this; UpdateUnits(); StartCoroutine(LoadTextures()); }
	private static Data Instance;
	public static Data GetInstance()
	{
		return Instance;
	}


	#region Units Here

	public GameObject UnitsContainer;
    public Color[] UnitColors;

    private Unit[] units;
    private List<Unit> unitList;

    public void UpdateUnits()
    {
        units = UnitsContainer.GetComponentsInChildren<Unit>();
        unitList = new List<Unit>(units);
    }

    public void AddUnit(Unit unit)
    {
        unitList.Add(unit);
		// Garbage collects old array, may think this out
		units = unitList.ToArray();
    }

    public void RemoveUnit(Unit unit)
    {
        unitList.Remove(unit);
		// Garbage collects old array, may think this out
        units = unitList.ToArray();
    }

    public Unit[] GetAllUnits()
    {
        // Should never ever return null units inside!!!!!
        return units; 
    }

	#endregion

	#region Skill textures here

	public Texture[] skillIconTextures;

	public IEnumerator LoadTextures()
	{
		// Load bundle
		var bundleLoadRequest = AssetBundle.LoadFromFileAsync("Assets/Asset Bundles/skill-icon-textures");
		yield return bundleLoadRequest;

		// Check if loaded correctly
		var myLoadedAssetBundle = bundleLoadRequest.assetBundle;
		if (myLoadedAssetBundle == null)
		{
			Debug.Log("Failed to load AssetBundle!");
			yield break;
		}

		// Load All assets
		var assetLoadRequest = myLoadedAssetBundle.LoadAllAssetsAsync<Texture>();
		yield return assetLoadRequest;

		Object[] texObjecs = assetLoadRequest.allAssets;
		skillIconTextures = new Texture[texObjecs.Length];
		foreach (var obj in texObjecs)
		{
			string[] num = obj.name.Split('_');
			skillIconTextures[int.Parse(num[num.Length - 1])] = obj as Texture;
		}

		myLoadedAssetBundle.Unload(false);
	}

	#endregion

	#region All Skill Instances here

	public Skill[] skills;

	#endregion
}
