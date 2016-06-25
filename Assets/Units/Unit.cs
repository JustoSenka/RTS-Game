using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

    private NavMeshAgent agent;
    private bool isHold = false;

	void Start () {
        agent = GetComponent<NavMeshAgent>();
	}
	
	void Update () {
	
	}

    public bool IsWaypointNecessary(Command command)
    {
        bool ret = false;
        switch (command)
        {
            case Command.Move:
            case Command.Attack:
            case Command.Skill0:
            case Command.Skill1:
                ret = true;
                break;
        }
        return ret;
    }

    public void PerformCommand(Command command, Vector3 target = default(Vector3), bool onSpecificUnit = false, Unit unit = null)
    {
        Debug.Log(command);

        switch (command)
        {
            case Command.Hold:
                isHold = true;
                agent.ResetPath();
                break;
            case Command.Stop:
                agent.ResetPath();
                break;
            case Command.Move:
                isHold = false;
                agent.SetDestination(target);
                break;
            case Command.Attack:
                isHold = false;
                agent.SetDestination(target);
                break;
        }
    }
}

public static class ExtensionMethods
{
    public static void PerformCommand(this List<Unit> units, Command command, Vector3 target = default(Vector3), bool onSpecificUnit = false, Unit unit = null)
    {
        foreach (var u in units)
        {
            u.PerformCommand(command, target, onSpecificUnit, unit);
        }
    }
}
