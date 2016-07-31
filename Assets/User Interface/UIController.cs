using System;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public IconPositionOnUI highlighter;
    public IconPositionOnUI holdHighlighter;
    public SelectRectangle selectRectangle;
    public InputControlTool inputControlTool;

    public RectTransform[] iconRects = new RectTransform[16];

    private int directClickOn = 0;
	private Unit lastUnitSelected;

	void Update ()
    {
        if (selectRectangle.GetSelectedUnits().Count > 0)
        {
			PutIconTexturesAccordingUnitSelected();
            EnableIcons(true);
            CheckIfClickedDirectlyOnUI();
            PutHighlightsAccordingInputControlTool();
            PutHighlightsOnDirectClick();
            HighlightHoldIcon();
        }
        else
        {
            EnableIcons(false);
            holdHighlighter.SetImageEnabled(false);
            highlighter.SetImageEnabled(false);
        }
    }

	private void PutIconTexturesAccordingUnitSelected()
	{
		if (lastUnitSelected != selectRectangle.GetSelectedUnits()[0])
		{
			lastUnitSelected = selectRectangle.GetSelectedUnits()[0];
			for (int i = 0; i < 4; i++)
			{
				iconRects[i].GetComponent<RawImage>().texture = lastUnitSelected.skills[i].main.tex;
			}
		}
	}

	private void EnableIcons(bool enabled)
    {
        var icon = iconRects[0].GetComponent<IconPositionOnUI>();
        if (icon.IsImageEnabled() == enabled)
            return;

        // TODO: may be slow, investigate is it possible to work out differently
        foreach (var icRect in iconRects)
        {
            icRect.GetComponent<IconPositionOnUI>().SetImageEnabled(enabled);
        }
    }

    private void CheckIfClickedDirectlyOnUI()
    {
        var currentCommand = GetCurrentCommandAccordingToMouse();
        if (!currentCommand.Equals(CommandType.None))
        {
            inputControlTool.PerformCommand(new Command(currentCommand));
        }
    }

    private CommandType GetCurrentCommandAccordingToMouse()
    {
        if (Input.GetMouseButton(0))
        {
            var pos = Input.mousePosition;
            foreach (var icon in iconRects)
            {
                if (icon.GetRect().Contains(pos))
                {
                    directClickOn = icon.GetComponent<IconPositionOnUI>().GetCell();
                    return icon.GetComponent<IconPositionOnUI>().commandType;
                }
            }
        }

        directClickOn = 0;
        return CommandType.None;
    }

    private void PutHighlightsAccordingInputControlTool()
    {
        bool highlightIconEnabled = true;

        if (!inputControlTool.IsInSkillPhase())
        {
            if (Input.GetButton("Skill0"))
            {
                highlighter.SetCell(9);
            }
            else if (Input.GetButton("Skill1"))
            {
                highlighter.SetCell(10);
			}
            else if (Input.GetButton("Skill2"))
            {
                highlighter.SetCell(11);
			}
            else if (Input.GetButton("Skill3"))
            {
                highlighter.SetCell(12);
			}
            else if (Input.GetButton("Attack"))
            {
                highlighter.SetCell(2);
			}
            else if (Input.GetButton("Hold"))
            {
                highlighter.SetCell(3);
			}
            else if (Input.GetButton("Stop"))
            {
                highlighter.SetCell(4);
			}
            else if (Input.GetButton("Move"))
            {
                highlighter.SetCell(1);
			}
            else
            {
                highlightIconEnabled = false;
            }
        }

        highlighter.SetImageEnabled(highlightIconEnabled);
    }

    private void PutHighlightsOnDirectClick()
    {
        if (directClickOn != 0)
        {
			highlighter.SetCell(directClickOn);
            highlighter.SetImageEnabled(true);
        }
    }

    private void HighlightHoldIcon()
    {
        bool isHold = false;

        Unit topUnit = (selectRectangle.GetSelectedUnits().Count > 0) ? selectRectangle.GetSelectedUnits()[0] : null;
        if (topUnit)
        {
            isHold = topUnit.IsHold();
        }

        holdHighlighter.SetImageEnabled(isHold);
    }
}
