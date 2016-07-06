using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    [System.NonSerialized]
	public bool showBar = true;
	public int barWidth = 80;
	public int barHeight = 8;
	public Color gradientTop = new Color(170f / 255f, 255f / 255f, 168f / 255f, 0.8f);
	public Color gradientBottom = new Color(3f / 255f, 80f / 255f, 15f / 255f, 0.8f);
	public Color topLeftBorder = new Color(0f, 0f, 0f, 0.8f);
	public Color bottomRightBorder = new Color(0f, 0f, 0f, 0.6f);

	public bool showText = true;
	public Color textColor = Color.red;
	public bool outlined = false;
	public Color outlineColor = Color.black;   
	public Font textFont = null;        
	public int fontSize = 12;             
	public FontStyle fontStyle = FontStyle.Bold; 

	private Unit unit;

	private Texture2D leftBar;
	private Texture2D rightBar;
	private Texture2D middleBar;
	private Texture2D emptyBar;
	private GUIStyle style;

	void Start()
	{
		unit = GetComponent<Unit>();

		leftBar = CreateLeftTexture();
		middleBar = CreateMiddleTexture();
		emptyBar = CreateEmptyTexture();
		rightBar = CreateEmptyTexture();

		style = new GUIStyle();
		style.normal.textColor = textColor;
		style.alignment = TextAnchor.UpperCenter;

		style.fontSize = fontSize;
		style.fontStyle = fontStyle;
		if (textFont != null)
			style.font = textFont;
		else
			style.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
	}

	void OnGUI()
	{
		if (!showBar)
			return;

		if (unit.IsDead())
			return;

		float current = 1f - ((unit.maxHp - unit.hp) / unit.maxHp);
        Camera camera = Camera.main;
        Vector3 cameraPosWorld = camera.transform.position;

        Vector3 pos = camera.WorldToScreenPoint(unit.pos + new Vector3(0, unit.height, 0));
		pos.y = Screen.height - pos.y;

		int calcBarWidth = (int)(barWidth * (15 / Vector3.Distance(cameraPosWorld, unit.pos)));
		int calcBarHeight = (int)(barHeight * (15 / Vector3.Distance(cameraPosWorld, unit.pos)));

        // This shit below has major performance issues on GPU,
        // Adds up to 600 draw calls with 100 minions with 20 ms both CPU and GPU usage !! 
        // Also 130 kb GC ALLOC

		Rect posBar = new Rect(pos.x - calcBarWidth / 2, pos.y, calcBarWidth * current, calcBarHeight);
		GUI.DrawTexture(new Rect((pos.x - 2) - (calcBarWidth / 2), pos.y, 2, calcBarHeight), leftBar);
		GUI.DrawTexture(new Rect(pos.x + (calcBarWidth / 2), pos.y, 2, calcBarHeight), rightBar);
		GUI.DrawTexture(posBar, middleBar);
		GUI.DrawTexture(new Rect(posBar.width + pos.x - calcBarWidth / 2, pos.y, calcBarWidth - posBar.width, calcBarHeight), emptyBar);
		if (showText)
		{
			var calcFontSize = (int)(fontSize * (15 / Vector3.Distance(cameraPosWorld, unit.pos)));
			style.fontSize = calcFontSize;
			string labelToDisplay = unit.hp + " / " + unit.maxHp;
			if (outlined)
				DrawOutline(new Rect(posBar.x, posBar.y - (calcFontSize + 2), calcBarWidth, 60), labelToDisplay, style, outlineColor, textColor);
			else
				GUI.Label(new Rect(posBar.x, posBar.y - (calcFontSize + 2), calcBarWidth, 60), labelToDisplay, style);
		}
	}

	private void DrawOutline(Rect rect, string text, GUIStyle theStyle, Color outColor, Color inColor)
	{
		var backupStyle = theStyle;
		theStyle.normal.textColor = outColor;
		rect.x--;
		GUI.Label(rect, text, style);
		rect.x += 2;
		GUI.Label(rect, text, style);
		rect.x--;
		rect.y--;
		GUI.Label(rect, text, style);
		rect.y += 2;
		GUI.Label(rect, text, style);
		rect.y--;
		theStyle.normal.textColor = inColor;
		GUI.Label(rect, text, style);
		theStyle = backupStyle;
	}

	// Create textures ----------------------------

	private Texture2D CreateLeftTexture()
	{
		return CreateColorTexture(new Rect(0, 0, 2, 16), topLeftBorder);
	}

	private Texture2D CreateMiddleTexture()
	{
		Rect rect = new Rect(0, 0, 1, 16);
		Texture2D result = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);
		int y = 0;
		while (y < result.height)
		{
			int x = 0;
			while (x < result.width)
			{
				if (y < 2)
					result.SetPixel(x, y, bottomRightBorder);
				else if (y > 13)
					result.SetPixel(x, y, topLeftBorder);
				else
				{
					Color color = new Color(gradientBottom.r - ((gradientBottom.r - gradientTop.r) / 14 * (y - 2)),
										   gradientBottom.g - ((gradientBottom.g - gradientTop.g) / 14 * (y - 2)),
										   gradientBottom.b - ((gradientBottom.b - gradientTop.b) / 14 * (y - 2)),
										   (gradientBottom.a + gradientTop.a) / 2f);
					result.SetPixel(x, y, color);
				}
				++x;
			}
			++y;
		}
		result.Apply();
		return result;
	}

	private Texture2D CreateEmptyTexture()
	{
		Rect rect = new Rect(0, 0, 1, 16);
		Texture2D result = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);
		int y = 0;
		while (y < result.height)
		{
			int x = 0;
			while (x < result.width)
			{
				if (y > 13)
					result.SetPixel(x, y, topLeftBorder);
				else
					result.SetPixel(x, y, bottomRightBorder);
				++x;
			}
			++y;
		}
		result.Apply();
		return result;
	}

	private Texture2D CreateColorTexture(Rect rect, Color color)
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
