using UnityEngine;
using UnityEditor;
using System.Collections;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class StretchRect : MonoBehaviour {

    public float originalWidth;
    public float originalHeight;
    private RectTransform transRect;

	void Start () {
        transRect = GetComponent<RectTransform>();
        SetSize();
    }

	void Update () {
#if UNITY_EDITOR
        SetSize();
#endif
    }

    private void SetSize()
    {
        if (transRect)
        {
            transRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.width * originalHeight / originalWidth);
        }

    }
}
