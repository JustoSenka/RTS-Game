using UnityEngine;
using System.Collections;

public class SelectProjector : MonoBehaviour
{
	public Projector projector;
	public bool scaleToObject = true;
	public bool scaleToBiggerAxis = true;
	public float circleSizeModification = 0;

	private Unit unit;

	void Start()
	{
		unit = GetComponent<Unit>();

		var go = unit.gameObject;
		if (scaleToObject)
		{
			var size = go.GetObjectSize();
			if (scaleToBiggerAxis) projector.orthographicSize = Mathf.Max(go.transform.localScale.x * size.x, go.transform.localScale.z * size.z);
			else projector.orthographicSize = Mathf.Min(go.transform.localScale.x * size.x, go.transform.localScale.z * size.z);
		}
		else
		{
			projector.orthographicSize = go.GetComponent<Unit>().radius + circleSizeModification;
		}
	}

	public void Enable()
	{
		if (!projector.enabled)
		{
			projector.enabled = true;
		}
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
