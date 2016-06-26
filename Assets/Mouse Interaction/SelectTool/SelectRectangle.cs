using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SelectRectangle : MonoBehaviour
{
    public enum MouseButton { left, right, middle }
    public MouseButton mouseButton = MouseButton.left;
    public KeyCode multiSelectKey = KeyCode.LeftShift;
    public bool scaleToObject = true;
    public bool scaleToBiggerAxis = true;
    public Color rectangleColor = new Color(190f / 255f, 190f / 255f, 1.0f, 0.2f);
    public Color rectangleBorderColor = new Color(0.5f, 0.5f, 1.0f, 0.9f);
    public GameObject onSelectUnit;
    public GameObject onDeselectUnit;
    public GameObject onStartSelect;
    public GameObject onEndSelect;
    public GameObject projectorPrefab;

    private List<Unit> selectedUnits { get; set; }
    private List<Unit> emptyList { get; set; }

    private Vector2 startPoint;
    private Texture2D rectTexture;
    private Texture2D borderTexture;
    private bool isDragging = false;

    public List<Unit> GetSelectedUnits()
    {
        if (isDragging)
            return emptyList;
        else
            return selectedUnits;
    }

    public void DeselectObject(GameObject go)
    {
        if (go.GetComponentsInChildren<Projector>().Length > 0)
        {
            Projector prj = go.GetComponentsInChildren<Projector>()[0];
            GameObject prnt = prj.transform.parent.gameObject;
            Destroy(prnt);
            selectedUnits.Remove(go.GetComponent<Unit>());
            //Send message if needed
            if (onDeselectUnit != null)
            {
                onSelectUnit.SendMessage("OnDeselectUnit", go);
            }
        }
    }

    void Awake()
    {
        rectTexture = CreateColorTexture(new Rect(0, 0, 2, 2), rectangleColor);
        borderTexture = CreateColorTexture(new Rect(0, 0, 2, 2), rectangleBorderColor);
        selectedUnits = new List<Unit>();
        emptyList = new List<Unit>();
    }

    private void DeselectAllObjects()
    {
        for (int i = 0; i < selectedUnits.Count; i++)
        {
            Projector projector = selectedUnits[i].GetComponentsInChildren<Projector>()[0];
            GameObject parent = projector.transform.parent.gameObject;
            Destroy(parent);
            //Send messages if needed
            if (onDeselectUnit != null)
            {
                onSelectUnit.SendMessage("OnDeselectUnit", selectedUnits[i]);
            }
        }
        selectedUnits.Clear();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(mouseButton.GetHashCode()))
        {
            isDragging = true;
            startPoint = Input.mousePosition;
            if (onStartSelect != null)
            {
                onSelectUnit.SendMessage("OnStartSelect", selectedUnits);
            }
            if (!Input.GetKey(multiSelectKey))
            {
                DeselectAllObjects();
            }
        }

        if (Input.GetMouseButtonUp(mouseButton.GetHashCode()) && isDragging)
        {
            isDragging = false;
            if (onEndSelect != null && selectedUnits.Count > 0)
            {
                onSelectUnit.SendMessage("OnEndSelect", selectedUnits);
            }
            if (Vector2.Distance(startPoint, Input.mousePosition) < 8)
            {
                if (!Input.GetKey(multiSelectKey))
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
    }

    private void PerformMouseClick()
    {
        IterateAllSelectable((GameObject go) =>
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit = new RaycastHit();

            if (Physics.Raycast(ray, out raycastHit, 100f))
            {
                if (raycastHit.collider.transform.gameObject.Equals(go))
                {
                    MarkObjectAsSelected(go);
                }
            }
        });
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
        if (go.GetComponentsInChildren<Projector>().Length == 0)
        {
            selectedUnits.Add(go.GetComponent<Unit>());
            GameObject slc = (GameObject)Instantiate(projectorPrefab);
            Projector prj = slc.GetComponentsInChildren<Projector>()[0];

            if (scaleToObject)
            {
                var size = GetObjectSize(go);
                if (scaleToBiggerAxis) prj.orthographicSize = Mathf.Max(go.transform.localScale.x * size.x, go.transform.localScale.z * size.z);
                else prj.orthographicSize = Mathf.Min(go.transform.localScale.x * size.x, go.transform.localScale.z * size.z);
            }
            else
            {
                prj.orthographicSize = 0.7f;
            }

            slc.transform.parent = go.transform;
            slc.transform.localPosition = new Vector3(0f, 0.1f, 0f);
            slc.transform.rotation = go.transform.rotation;

            //Send message if needed
            if (onSelectUnit != null)
            {
                onSelectUnit.SendMessage("OnSelectUnit", go);
            }
        }
    }

    private Vector3 GetObjectSize(GameObject go)
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
