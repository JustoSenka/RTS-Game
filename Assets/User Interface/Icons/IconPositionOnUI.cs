using UnityEngine;
using System.Collections;
using UnityEngine.UI;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class IconPositionOnUI : MonoBehaviour
{
    public RectTransform skillImage;

	[Range(1, 12)]
	[SerializeField]
	private int cell = 1;

	public float sizeInCell;
    public CommandType commandType;

	public bool updateEveryFrame = false;

	[Range(0, 1)]
	public float heightModifier = 1;

	private RectTransform transRect;
    private RawImage rawImage;
    private StretchRect skillImageStretch;

    private readonly float startX = 204;
    private readonly float startY = 142;
    private readonly float stepX = 54.8f;
    private readonly float stepY = 55;

    void Start()
    {
        transRect = GetComponent<RectTransform>();
        rawImage = GetComponent<RawImage>();
        skillImageStretch = skillImage.GetComponent<StretchRect>();
		SetSize();
		this.RunAfter(0.06f, () => SetSize());
	}

    void Update()
    {
#if UNITY_EDITOR
        SetSize();
#else
		if (updateEveryFrame)
		{
			SetSize();
		}
#endif
    }

	// Only works when pivot is (0.5, 0.5)
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

        float size = Mathf.Min(stepX, stepY) * sizeInCell * scaleFactor;
        transRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size * heightModifier);
        transRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);

		transRect.position = new Vector3(posX, posY - size * (1 - heightModifier) * 0.5f, 0);
	}

	public void SetCell(int cell)
	{
		this.cell = cell;
		SetSize();
	}

	public int GetCell()
	{
		return cell;
	}

    public void SetImageEnabled(bool enabled)
    {
		SetSize();
		if (rawImage.enabled != enabled)
        {
            rawImage.enabled = enabled;
        }
    }

    public bool IsImageEnabled()
    {
        return rawImage.enabled;
    }
}
