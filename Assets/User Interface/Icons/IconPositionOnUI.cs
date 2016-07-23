using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class IconPositionOnUI : MonoBehaviour
{
    public RectTransform skillImage;

    [Range(1, 12)]
    public int cell = 1;
    public float sizeInCell;

    private RectTransform transRect;
    private StretchRect skillImageStretch;

    private readonly float startX = 204;
    private readonly float startY = 142;
    private readonly float stepX = 54.8f;
    private readonly float stepY = 55;

    void Start()
    {
        transRect = GetComponent<RectTransform>();
        skillImageStretch = skillImage.GetComponent<StretchRect>();
        SetSize();
    }

    void Update()
    {
#if UNITY_EDITOR
        SetSize();
#endif
    }

    private void SetSize()
    {
        float posX = 0;
        float posY = 0;
        
        if (!skillImageStretch) skillImageStretch = skillImage.GetComponent<StretchRect>();
        float scaleFactor = skillImage.rect.height / skillImageStretch.originalHeight;

        if (transRect)
        {
            posX = Screen.width - (startX - stepX * ((cell - 1) % 4)) * scaleFactor;
            posY = (startY - (stepY * (Mathf.CeilToInt((cell - 1) / 4)))) * scaleFactor;
        }
        
        transRect.position = new Vector3(posX, posY, 0);

        float size = Mathf.Min(stepX, stepY) * sizeInCell * scaleFactor;
        transRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
        transRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
    }
}
