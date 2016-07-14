using UnityEngine;
using System.Collections;

public class TeamColor : MonoBehaviour {

    public Unit unit;
    private Team team;
    private Renderer render;

    private bool firstFrame = false;

	void Start () {
        render = GetComponent<Renderer>();
        team = unit.team;

        foreach (var mat in render.materials)
        {
            mat.color = Data.GetInstance().UnitColors[team.GetHashCode()];
        }
    }
	
	void Update ()
    {
        if (!unit.team.Equals(team))
        {
            team = unit.team;
            foreach (var mat in render.materials)
            {
                mat.color = Data.GetInstance().UnitColors[team.GetHashCode()];
            }
        }
    }
}
