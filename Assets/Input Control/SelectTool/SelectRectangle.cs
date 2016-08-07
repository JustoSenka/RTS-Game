using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SelectRectangle : MonoBehaviour
{
    public Team team = Team.T1;

    [Header("Mouse Buttons:")]
    public MouseButton mouseButton = MouseButton.Left;
    public KeyCode multiSelectKey = KeyCode.LeftShift;
    [Space(5)]

    [Header("Selection Rectangle Options:")]
    public Color rectangleColor = new Color(190f / 255f, 190f / 255f, 1.0f, 0.2f);
    public Color rectangleBorderColor = new Color(0.5f, 0.5f, 1.0f, 0.9f);
    [Space(5)]

    public RectTransform[] excludedScreenAreas;

	private SortedList<Unit> selectedUnits;
	private SortedList<Unit> emptyList;

	private Vector2 startPoint;
    private Texture2D rectTexture;
    private Texture2D borderTexture;
    private bool isDragging = false;

    public SortedList<Unit> GetSelectedUnits()
    {
        if (isDragging)
            return emptyList;
        else
            return selectedUnits;
    }

    public void DeathCallback(GameObject unitGO)
    {
        DeselectObject(unitGO);
    }

    public void DeselectObject(GameObject go)
    {
		var projector = go.GetComponent<SelectProjector>();
		if (projector)
		{
			projector.Disable();
		}

		selectedUnits.Remove(go.GetComponent<Unit>());
    }

    void Awake()
    {
        rectTexture = CreateColorTexture(new Rect(0, 0, 2, 2), rectangleColor);
        borderTexture = CreateColorTexture(new Rect(0, 0, 2, 2), rectangleBorderColor);
        selectedUnits = new SortedList<Unit>(new UnitComparer());
        emptyList = new SortedList<Unit>(new UnitComparer());
    }

    private void DeselectAllObjects()
    {
		foreach (var unit in selectedUnits)
		{
			var projector = unit.GetComponent<SelectProjector>();
			if (projector)
			{
				projector.Disable();
			}
		}

        selectedUnits.Clear();
    }

    void Update()
    {
        // Released mouse while dragged, confirm selection
        if (Input.GetMouseButtonUp(mouseButton.GetHashCode()) && isDragging)
        {
            isDragging = false;
            if (Vector2.Distance(startPoint, Input.mousePosition) < 8)
            {
                if (!Input.GetKey(multiSelectKey) && !excludedScreenAreas.Contains(Input.mousePosition))
                {
                    DeselectAllObjects();
                }
                PerformMouseClick();
            }
            else if (selectedUnits.Count == 0)
            {
                PerformMouseClick();
            }
        }

        // Check if started dragging
        if (Input.GetMouseButtonDown(mouseButton.GetHashCode()) && !excludedScreenAreas.Contains(Input.mousePosition))
        {
            isDragging = true;
            startPoint = Input.mousePosition;
            if (!Input.GetKey(multiSelectKey))
            {
                DeselectAllObjects();
            }
        }

        // Activelly select all units under selection rect
        if (isDragging)
        {
            if (Vector2.Distance(startPoint, Input.mousePosition) < 8)
                return;

            Rect rect = ConstructSelectionRect();
            IterateAllSelectable((GameObject go) =>
            {
                Vector3 pos = Camera.main.WorldToScreenPoint(go.transform.position);
                pos.y = Screen.height - pos.y;

                if (rect.Contains(new Vector2(pos.x, pos.y)))
                {
                    MarkObjectAsSelected(go);
                }
                else if (!Input.GetKey(multiSelectKey))
                {
                    DeselectObject(go);
                }
            });
        }

#if UNITY_EDITOR
		// Regression test for bug when same object can be selected multiple times
		if (GetSelectedUnits().Count > Data.GetInstance().GetAllUnits().Length)
		{
			Debug.LogError("Selected unit count is greater than total unit count on field: "
				+ GetSelectedUnits().Count + " > " + Data.GetInstance().GetAllUnits().Length);
		}
#endif
	}

    private void PerformMouseClick()
    {
        var go = Common.GetObjectUnderMouse();
        var unit = (go) ? go.GetComponent<Unit>() : null;
        if (unit != null && unit.team.Equals(team))
        {
            MarkObjectAsSelected(unit.gameObject);
        }
    }

    private void IterateAllSelectable(Action<GameObject> action)
    {
        try
        {
            GameObject[] allObjects = GameObject.FindGameObjectsWithTag("Selectable");
            foreach (GameObject go in allObjects)
            {
                action(go);
            }
        }
        catch (UnityException e)
        {
            Debug.Log(e.Message);
            Debug.LogWarning("ATTENTION!! Create a tag named 'Selectable' to avoid this errors!!");
        }
    }

    private Rect ConstructSelectionRect()
    {
        Rect rect = new Rect(startPoint.x, Screen.height - startPoint.y, Input.mousePosition.x - startPoint.x, startPoint.y - Input.mousePosition.y);
        if (rect.width < 0)
        {
            rect.x += rect.width;
            rect.width *= -1;
        }
        if (rect.height < 0)
        {
            rect.y += rect.height;
            rect.height *= -1;
        }
        return rect;
    }

    private void MarkObjectAsSelected(GameObject go)
    {
        var unit = go.GetComponent<Unit>();
        if (!unit.team.Equals(team) || unit.IsDead())
            return;

		var projector = go.GetComponent<SelectProjector>();
		if (projector && !projector.IsEnabled())
        {
            selectedUnits.Add(unit);
			projector.Enable();
        }
    }

    //Draws the select rectangle on screen
    void OnGUI()
    {
        if (isDragging)
        {
            Rect rect = new Rect(startPoint.x, Screen.height - startPoint.y, Input.mousePosition.x - startPoint.x, startPoint.y - Input.mousePosition.y);
            GUI.DrawTexture(rect, rectTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, 1), borderTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y, 1, rect.height), borderTexture);
            GUI.DrawTexture(new Rect(rect.x + rect.width, rect.y, 1, rect.height), borderTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y + rect.height, rect.width, 1), borderTexture);
        }
    }

    Texture2D CreateColorTexture(Rect rect, Color color)
    {
        Texture2D result = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);
        int y = 0;
        while (y < result.height)
        {
            int x = 0;
            while (x < result.width)
            {
                result.SetPixel(x, y, color);
                ++x;
            }
            ++y;
        }
        result.Apply();
        return result;
    }
}

public enum MouseButton
{
    Left, Right, Middle
}
