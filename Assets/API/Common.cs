using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Common {


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
