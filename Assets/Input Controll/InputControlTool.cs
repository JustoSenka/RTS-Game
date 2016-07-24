using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class InputControlTool : MonoBehaviour
{
    public LayerMask groundLayer;

    private IEnumerable<CommandType> allCommands = Enum.GetValues(typeof(CommandType)).Cast<CommandType>();

    private bool skillPhase = false;
    private Command currentCommand;

    private SelectRectangle selectRectangle;
    private TargetMoveAnimation moveCross;

    void Start()
    {
        selectRectangle = gameObject.GetComponent<SelectRectangle>();
        moveCross = gameObject.GetComponentInChildren<TargetMoveAnimation>();
    }

    void Update()
    {
        var selectedUnits = selectRectangle.GetSelectedUnits();

        if (selectedUnits.Count > 0)
        {
            // Checks if skill or special keys are pressed
            if (!skillPhase)
            {
                PerformCommand(GetCurrentCommandAccordingToInput());
            }

            if (selectRectangle.excludedScreenAreas.Contains(Input.mousePosition))
                return;

            // Perform actions
            if (skillPhase && Input.GetMouseButtonDown(MouseButton.Left.GetHashCode()))
            {
                SetSelectionEnabled(true);
                skillPhase = false;

                // Attack or skill on any unit
                var unitClickedOn = Common.GetObjectUnderMouse().GetComponent<Unit>();
                if (unitClickedOn != null && !currentCommand.type.Equals(CommandType.Move))
                {
                    selectedUnits.PerformCommand(new Command(currentCommand.type, unitClickedOn.transform.position, unitClickedOn, true));
                    moveCross.ShowAt(unitClickedOn.transform.position, true);
                }
                // Attack or skill on ground
                else
                {
                    var mousePos = Common.GetWorldMousePoint(groundLayer);
                    moveCross.ShowAt(mousePos, !currentCommand.type.Equals(CommandType.Move));
                    currentCommand.pos = mousePos;
                    selectedUnits.PerformCommand(currentCommand);
                }
            }
            else if (skillPhase && Input.GetMouseButtonDown(MouseButton.Right.GetHashCode()))
            {
                SetSelectionEnabled(true);
                currentCommand = new Command(CommandType.None);
                skillPhase = false;
            }
            else if (!skillPhase && Input.GetMouseButtonDown(MouseButton.Right.GetHashCode()))
            {
				// Right click on enemy unit
				var obj = Common.GetObjectUnderMouse();
				if (obj != null)
				{
					var unitClickedOn = obj.GetComponent<Unit>();
					if (unitClickedOn != null && !unitClickedOn.team.Equals(selectRectangle.team))
					{
                        selectedUnits.PerformCommand(new Command(CommandType.Attack, unitClickedOn.transform.position, unitClickedOn, true));
						moveCross.ShowAt(unitClickedOn.transform.position, true);
					}
					// Right click on ground or anywhere else
					else
					{
						var mousePos = Common.GetWorldMousePoint(groundLayer);
						moveCross.ShowAt(mousePos);
                        selectedUnits.PerformCommand(new Command(CommandType.Move, mousePos));
					}
				}
            }
        }
    }

    public void PerformCommand(Command commandToPerform)
    {
        currentCommand = commandToPerform;
        skillPhase = !currentCommand.type.Equals(CommandType.None);
        SetSelectionEnabled(!skillPhase);

        if (!currentCommand.type.Equals(CommandType.None) && !selectRectangle.GetSelectedUnits()[0].IsWaypointNecessary(currentCommand))
        {
            selectRectangle.GetSelectedUnits().PerformCommand(currentCommand);
            skillPhase = false;
        }
    }

    private Command GetCurrentCommandAccordingToInput()
    {
        foreach (var c in allCommands)
        {
            if (!c.Equals(CommandType.None) && !c.Equals(CommandType.Busy) && Input.GetButtonDown(c.ToString()))
            {
                return new Command(c);
            }
        }
        return new Command(CommandType.None);
    }

    public void SetSelectionEnabled(bool enabled)
    {
        if (selectRectangle != null)
        {
            selectRectangle.enabled = enabled;
        }
    }

    public bool IsInSkillPhase()
    {
        return skillPhase;
    }

    private enum MouseButton { Left, Right, Midle }
}


