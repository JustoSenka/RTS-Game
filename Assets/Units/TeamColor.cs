using UnityEngine;
using System.Collections;

public class TeamColor : MonoBehaviour {

    public Unit unit;
    private Team team;
    private Renderer render;

	private MaterialPropertyBlock props;
	void Start () {
        render = GetComponent<Renderer>();
        team = unit.team;
		
		props = new MaterialPropertyBlock();
		props.SetColor("_Color", Data.GetInstance().UnitColors[team.GetHashCode()]);
		render.SetPropertyBlock(props);
    }
	
	void Update ()
    {
        if (!unit.team.Equals(team))
        {
            team = unit.team;

			props.SetColor("_Color", Data.GetInstance().UnitColors[team.GetHashCode()]);
			render.SetPropertyBlock(props);
        }
    }
}
