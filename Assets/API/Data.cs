using UnityEngine;
using System.Collections;

public class Data : MonoBehaviour
{
    public GameObject UnitsContainer;

    private Unit[] units;
    private Transform[] unitTransforms;
    private Vector3[] unitPositions;

    private float lastUpdate;
    private readonly float updateInterval = 0.1f;

    void Start() { Instance = this; }
    private static Data Instance;
    public static Data GetInstance()
    {
        return Instance;
    }

    public Unit[] GetAllUnits()
    {
        if (units == null || Time.time >= lastUpdate + updateInterval)
        {
            lastUpdate = Time.time;
            units = UnitsContainer.GetComponentsInChildren<Unit>();
        }
        return units; 
    }

    public Transform[] GetAllUnitTransforms()
    {
        if (unitTransforms == null || Time.time >= lastUpdate + updateInterval)
        {
            lastUpdate = Time.time;
            unitTransforms = UnitsContainer.GetComponentsInChildren<Transform>();
        }
        return unitTransforms;
    }

    public Vector3[] GetAllUnitPositions()
    {
        if (unitTransforms == null || Time.time >= lastUpdate + updateInterval)
        {
            lastUpdate = Time.time;
            var trans = GetAllUnitTransforms();
            unitPositions = new Vector3[trans.Length];
            for (int i = 0; i < trans.Length; i++)
            {
                unitPositions[i] = trans[i].position;
            }
        }
        return unitPositions;
    }
}
