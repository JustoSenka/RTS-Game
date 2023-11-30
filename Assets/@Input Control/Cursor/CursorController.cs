using UnityEngine;
using System.Collections;
using System;

public class CursorController : MonoBehaviour {
	[Header("Cursor textures here: 128x128")]
	public Texture2D defaultCursor;
	public Texture2D attackCursor;
	public Texture2D attackCursorHighlight;
	public Texture2D magicCursor;
	public Texture2D magicCursorHighlight;

	[Header("Hotspot offsets")]
	public Vector2 defaultCursorHotspot = new Vector2(3 / 16.0f * 128, 3 / 16.0f * 128);
	public Vector2 attackCursorHotspot = new Vector2(64f, 64f);
	public Vector2 magicCursorHotspot = new Vector2(64f, 64f);

	[Header("Public references to GameControler")]
	public InputControlTool inputControlTool;
	public SelectRectangle selectRectangle;

	private CursorType currentCursor;
	private float timeDisabled = 0;

	void Start ()
	{
		Cursor.SetCursor(defaultCursor, defaultCursorHotspot, CursorMode.Auto);
		currentCursor = CursorType.Default;

		// Lets make sure we have all the dependencies, even if not set in the inspector :)
		if (!inputControlTool)
			inputControlTool = GetComponent<InputControlTool>();

		if (!inputControlTool)
			inputControlTool = GameObject.FindGameObjectWithTag("GameControler").GetComponent<InputControlTool>();

		if (!selectRectangle)
			selectRectangle = GetComponent<SelectRectangle>();

		if (!selectRectangle)
			selectRectangle = GameObject.FindGameObjectWithTag("GameControler").GetComponent<SelectRectangle>();
	}
	
	void Update ()
	{
		if (timeDisabled <= 0)
		{
			var cursorType = CheckWhatCursorShouldBe();
			ApplyCursorIfChanged(cursorType);
		}
		else
			timeDisabled -= Time.deltaTime;
	}

	private CursorType CheckWhatCursorShouldBe()
	{
		var cursorType = CursorType.Default;
		
		Unit unit = null;
		var go = Common.GetObjectUnderMouse(true);
		if (go) unit = go.GetComponent<Unit>();

		// From input control tool
		if (inputControlTool.IsInSkillPhase())
		{
			if (inputControlTool.GetCurrentCommand().IsAttack())
			{
				if (unit && unit.team.Equals(selectRectangle.team))
					cursorType = CursorType.AttackHighlight;
				else
					cursorType = CursorType.Attack;
			}
			else if (inputControlTool.GetCurrentCommand().IsSkill())
			{
				cursorType = CursorType.Magic;
			}
		}

		// If mouse is over enemy unit show attack icon
		else if (selectRectangle.GetSelectedUnits().Count > 0)
		{
			if (unit && !unit.team.Equals(selectRectangle.team))
			{
				cursorType = CursorType.Attack;
			}
		}

		// if pressing mouse, show highlight
		if (inputControlTool.IsInSkillPhase() && Input.GetKey(KeyCode.Mouse0) ||
			!inputControlTool.IsInSkillPhase() && Input.GetKey(KeyCode.Mouse1))
		{
			timeDisabled = 0.05f;
			if (cursorType.Equals(CursorType.Attack))
				cursorType = CursorType.AttackHighlight;
			if (cursorType.Equals(CursorType.Magic))
				cursorType = CursorType.MagicHighlight;
		}

		return cursorType;
	}

	private void ApplyCursorIfChanged(CursorType cursorType)
	{
		if (cursorType.Equals(currentCursor))
			return;

		currentCursor = cursorType;
		Cursor.SetCursor(GetTextureFromType(currentCursor), GetHotspotFromType(currentCursor), CursorMode.Auto);
	}

	private Texture2D GetTextureFromType(CursorType currentCursor)
	{
		switch (currentCursor)
		{
			case CursorType.Attack:
				return attackCursor;
			case CursorType.Magic:
				return magicCursor;
			case CursorType.AttackHighlight:
				return attackCursorHighlight;
			case CursorType.MagicHighlight:
				return magicCursorHighlight;
			default:
				return defaultCursor;
		}
	}

	private Vector2 GetHotspotFromType(CursorType currentCursor)
	{
		switch (currentCursor)
		{
			case CursorType.Attack:
			case CursorType.AttackHighlight:
				return attackCursorHotspot;

			case CursorType.Magic:
			case CursorType.MagicHighlight:
				return magicCursorHotspot;

			default:
				return defaultCursorHotspot;
		}
	}

	private enum CursorType
	{
		Default, Attack, Magic, AttackHighlight, MagicHighlight
	}
}
