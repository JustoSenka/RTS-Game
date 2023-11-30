using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class StretchRect : MonoBehaviour {

    public float originalWidth;
    public float originalHeight;

    public StretchType stretchType = StretchType.Width;
    public bool keepWidthHeightRatio = true;
    public bool fitToScreen = true;

    public float preferredHeightPercentage;
    public float preferredWidthPercentage;

    private RectTransform transRect;
    private float width;
    private float height;

	void Start () {
        transRect = GetComponent<RectTransform>();
        SetSize();
		this.RunAfter(0.04f, () => SetSize());
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
            SetSizeAccordingToStretchType();

            if (fitToScreen)
            {
                SetSizeToFitScreen();
            }

            if (keepWidthHeightRatio)
            {
                transRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                transRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            }
            else if (stretchType.Equals(StretchType.Width))
            {
                transRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            }
            else
            {
                transRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
        }
    }

    private void SetSizeAccordingToStretchType()
    {
        if (stretchType.Equals(StretchType.Width))
        {
            width = Screen.width * preferredWidthPercentage;
            height = originalHeight * (width / originalWidth);
        }
        if (stretchType.Equals(StretchType.Height))
        {
            height = Screen.height * preferredHeightPercentage;
            width = originalWidth * (height / originalHeight);
        }
    }

    private void SetSizeToFitScreen()
    {
        if (height > Screen.height)
        {
            height = Screen.height;
            width = originalWidth * (height / originalHeight);
        }
        if (width > Screen.width)
        {
            width = Screen.width;
            height = originalHeight * (width / originalWidth);
        }
    }
}



public enum StretchType { Height, Width }