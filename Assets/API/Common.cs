using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Common
{
    public static GameObject GetObjectUnderMouse()
    {
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f))
        {
            return hit.collider.gameObject;
        }

        Debug.LogWarning("There is no object under the mouse.");
        return null;
    }

    public static Vector3 GetWorldMousePoint(LayerMask layerMask)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f, layerMask))
        {
            return hit.point;
        }

        Debug.LogWarning("Did not clicked on the layer.");
        return Vector3.zero;
    }
}



public static class ExtensionMethods
{
    public static void PerformCommand(this List<Unit> units, Command command)
    {/*
		if (units.Count > 1 && (command.Equals(Command.Move) || (command.Equals(Command.Attack) && unit == null)))
		{
			int cols = Mathf.RoundToInt(Mathf.Sqrt(units.Count));

			//TODO: construct a grid of units
		}
		else
		{*/
        foreach (var u in units)
        {
            u.PerformCommand(command);
        }
        //}
    }

    public static float GetBiggestUnitRadius(this List<Unit> units)
    {
        float max = 0;
        foreach (var u in units)
        {
            max = (max > u.radius) ? max : u.radius;
        }
        return max;
    }

    public static NavMeshAgent GetClosestAgent(this NavMeshAgent[] agents, Vector3 pos)
    {
        float minDist = Vector3.Distance(agents[0].transform.position, pos);
        NavMeshAgent closestAgent = agents[0];
        foreach (var a in agents)
        {
            if (minDist > Vector3.Distance(a.transform.position, pos))
            {
                closestAgent = a;
                minDist = Vector3.Distance(a.transform.position, pos);
            }
        }
        return closestAgent;
    }

    public static Vector3 GetObjectSize(this GameObject go)
    {
        Vector3 size = new Vector3(1, 1, 1);
        //First look for SkinnedMeshRenderer
        SkinnedMeshRenderer[] meshes = go.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (meshes.Length > 0)
        {
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
            foreach (SkinnedMeshRenderer r in meshes)
            {
                bounds.Encapsulate(r.sharedMesh.bounds);
            }
            size = bounds.size;
        }
        //If no skinned, look for mesh filters
        else
        {
            MeshFilter[] meshesf = go.GetComponentsInChildren<MeshFilter>();
            if (meshesf.Length > 0)
            {
                Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
                foreach (MeshFilter r in meshesf)
                {
                    bounds.Encapsulate(r.mesh.bounds);
                }
                size = bounds.size;
            }
        }
        return size;
    }

    public static void RunAfter(this MonoBehaviour mono, float sec, Action ac)
    {
        mono.StartCoroutine(RunAfterEnum(sec, ac));
    }

    public static IEnumerator RunAfterEnum(float sec, Action ac)
    {
        yield return new WaitForSeconds(sec);
        ac.Invoke();
    }
}
