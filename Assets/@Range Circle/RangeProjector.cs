using UnityEngine;
using System.Collections;

public class RangeProjector : MonoBehaviour
{
	public Projector projector;

	[System.NonSerialized]
	public float radius;

	private Unit unit;

	void Start()
	{
		unit = GetComponent<Unit>();
		var go = unit.gameObject;
	}

	public void Enable(float radius = 0)
	{
		if (!projector.enabled)
		{
			projector.enabled = true;
		}
		projector.orthographicSize = (radius == 0) ? this.radius : radius;
	}

	public void Disable()
	{
		if (projector.enabled)
		{
			projector.enabled = false;
		}
	}

	public bool IsEnabled()
	{
		return projector.enabled;
	}
}
