using UnityEngine;
using System.Collections;

public class LiveBar : MonoBehaviour {

	public bool showBar = true;
	public int totalLive = 100;
	public int currentLive = 100;
	public int barWidth = 80;
	public int barHeight = 10;
	public float yDelta=0f;
	public Color gradientTop=new Color(170f/255f, 255f/255f, 168f/255f, 0.8f);
	public Color gradientBottom=new Color(3f/255f, 80f/255f, 15f/255f, 0.8f);
	public Color topLeftBorder=new Color(0f, 0f, 0f, 0.8f);
	public Color bottomRightBorder=new Color(0f, 0f, 0f, 0.6f);

	public bool showText = true;
	public Color labelColor=Color.red;	//Text color
	public bool outlined=false;				//Set true for display outlined text
	public Color outlineColor=Color.black;	//Outline text color
	public Font textFont = null;			//Text font
	public int fontSize=12;					//Text size
	public FontStyle fontStyle=FontStyle.Bold; //Font style

	private Texture2D leftBar;
	private Texture2D rightBar;
	private Texture2D middleBar;
	private Texture2D emptyBar;
	private GUIStyle style;

	//Use this public functions to control the live bar
	//Full compatible with PlayMaker event system

	//Show the live bar
	public void showLiveBar() {
		showBar = true;
	}

	//Hides live bar
	public void hideLiveBar() {
		showBar = false;
	}

	//Show text in liver bar
	public void showTextBar() {
		showText = true;
	}

	//Hides text in live bar
	public void hideTextBar() {
		showText = false;
	}

	//Set live on percentage
	//Values are clipped between 0 and totalLive
	public void setLiveInPercentage(float percenge) {
		currentLive = (int)((float)totalLive * percenge / 100.0f);
		if(currentLive>totalLive)
			currentLive=totalLive;
		if(currentLive<0)
			currentLive=0;
	}

	//Set live in absolute value
	//Values are clipped between 0 and totalLive
	public void setLive(int live) {
		currentLive = live;
		if(currentLive>totalLive)
			currentLive=totalLive;
		if(currentLive<0)
			currentLive=0;
	}

	//Decreases live in an absolute value
	public void decreaseLive(int liveToDecrease) {
		currentLive -= liveToDecrease;
		if(currentLive<0)
			currentLive=0;
	}

	//Decreases live in an percentage value
	public void decreaseLiveInPercentage(float percengeToDecrease) {
		int liveToDecrease = (int)((float)totalLive * percengeToDecrease / 100.0f);
		decreaseLive(liveToDecrease);
	}

	//Increases live in an absolute value
	public void increaseLive(int liveToIncrease) {
		currentLive += liveToIncrease;
		if(currentLive>totalLive)
			currentLive=totalLive;
	}
	
	//Increases live in an percentage value
	public void increaseLiveInPercentage(float percengeToIncrease) {
		int liveToIncrease = (int)((float)totalLive * percengeToIncrease / 100.0f);
		increaseLive(liveToIncrease);
	}

	//Get current live
	public int getCurrentLive() {
		return currentLive;
	}

	//Set total live value
	public void setTotalLive(int total) {
		totalLive = total;
	}

	//Get total live value
	public int getTotalLive() {
		return totalLive;
	}

	void Awake() {
		leftBar = CreateLeftTexture ();
		middleBar = CreateMiddleTexture ();
		emptyBar = CreateEmptyTexture ();
		rightBar = CreateEmptyTexture ();

		style = new GUIStyle();
		style.normal.textColor = labelColor;  
		style.alignment = TextAnchor.UpperCenter;
		style.wordWrap = true;
		style.fontSize = fontSize;
		style.fontStyle = fontStyle;
		if(textFont!=null)
			style.font=textFont;
		else
			style.font= (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
	}

	Texture2D CreateLeftTexture() {
		return CreateColorTexture (new Rect (0, 0, 2, 16), topLeftBorder);
	}

	Texture2D CreateMiddleTexture() {
		Rect rect = new Rect (0, 0, 1, 16);
		Texture2D result = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);
		int y = 0;
		while (y < result.height) {
			int x = 0;
			while (x < result.width) {
				if(y<2)
					result.SetPixel(x, y, bottomRightBorder);
				else if(y>13)
					result.SetPixel(x, y, topLeftBorder);
				else {
					Color color= new Color(gradientBottom.r-((gradientBottom.r-gradientTop.r)/14*(y-2)),
					                       gradientBottom.g-((gradientBottom.g-gradientTop.g)/14*(y-2)),
					                       gradientBottom.b-((gradientBottom.b-gradientTop.b)/14*(y-2)),
					                       (gradientBottom.a+gradientTop.a)/2f);
					result.SetPixel(x, y, color);
				}
				++x;
			}
			++y;
		}
		result.Apply();
		return result;
	}

	Texture2D CreateEmptyTexture() {
		Rect rect = new Rect (0, 0, 1, 16);
		Texture2D result = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);
		int y = 0;
		while (y < result.height) {
			int x = 0;
			while (x < result.width) {
				if(y>13)
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

	Texture2D CreateColorTexture(Rect rect, Color color) {
		Texture2D result = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);
		int y = 0;
		while (y < result.height) {
			int x = 0;
			while (x < result.width) {
				result.SetPixel(x, y, color);
				++x;
			}
			++y;
		}
		result.Apply();
		return result;
	}

	void OnGUI() {
		if (!showBar)
			return;
		float current =1f-( ((float)totalLive - (float)currentLive) / (float)totalLive);
		Vector3 posDelta = transform.position;
		posDelta.y += yDelta;
		Vector3 pos = Camera.main.WorldToScreenPoint(posDelta);
		pos.y = Screen.height - pos.y;

		Rect posLeft = new Rect((pos.x-2)-(barWidth/2), pos.y, 2, barHeight);
		Rect posRight = new Rect(pos.x+(barWidth/2), pos.y, 2, barHeight);
		Rect posBar = new Rect(pos.x-barWidth/2, pos.y, barWidth*current, barHeight);
		Rect posEmpty = new Rect(posBar.width+pos.x-barWidth/2, pos.y, barWidth-posBar.width, barHeight);
		GUI.DrawTexture (posLeft, leftBar);
		GUI.DrawTexture (posRight, rightBar);
		GUI.DrawTexture (posBar, middleBar);
		GUI.DrawTexture (posEmpty, emptyBar);
		if (showText) {
			string labelToDisplay=currentLive+" / "+totalLive;
			if (outlined)
				DrawOutline (new Rect(posBar.x,posBar.y-(fontSize+2),barWidth,60), labelToDisplay, style, outlineColor, labelColor);
			else
				GUI.Label ( new Rect(posBar.x,posBar.y-(fontSize+2),barWidth,60), labelToDisplay, style);
		}
	}

	//draw text of a specified color, with a specified outline color
	void DrawOutline(Rect position, string text, GUIStyle theStyle, Color outColor, Color inColor){
		var backupStyle = theStyle;
		theStyle.normal.textColor = outColor;
		position.x--;
		GUI.Label(position, text, style);
		position.x +=2;
		GUI.Label(position, text, style);
		position.x--;
		position.y--;
		GUI.Label(position, text, style);
		position.y +=2;
		GUI.Label(position, text, style);
		position.y--;
		theStyle.normal.textColor = inColor;
		GUI.Label(position, text, style);
		theStyle = backupStyle;
	}

}
