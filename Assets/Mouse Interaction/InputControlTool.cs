using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class InputControlTool : MonoBehaviour
{
    public LayerMask groundLayer;

    private IEnumerable<Command> allCommands = Enum.GetValues(typeof(Command)).Cast<Command>();

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
        if (selectRectangle.GetSelectedUnits().Count > 0)
        {
            // Checks if skill or special keys are pressed
            if (!skillPhase)
            {
                currentCommand = GetCurrentCommandAccordingToInput();
                skillPhase = !currentCommand.Equals(Command.None);
                SetSelectionEnabled(!skillPhase);

                if (!currentCommand.Equals(Command.None) && !selectRectangle.GetSelectedUnits()[0].IsWaypointNecessary(currentCommand))
                {
                    selectRectangle.GetSelectedUnits().PerformCommand(currentCommand);
                    skillPhase = false;
                }
            }

            // Perform actions
            if (skillPhase && Input.GetMouseButtonDown(0))
            {
                SetSelectionEnabled(true);
                skillPhase = false;

                moveCross.ShowAt(GetWorldMousePoint(), !currentCommand.Equals(Command.Move));
                selectRectangle.GetSelectedUnits().PerformCommand(currentCommand, GetWorldMousePoint());
            }
            else if (skillPhase && Input.GetMouseButtonDown(1))
            {
                SetSelectionEnabled(true);
                skillPhase = false;
            }
            else if (!skillPhase && Input.GetMouseButtonDown(1))
            {
                moveCross.ShowAt(GetWorldMousePoint());
                selectRectangle.GetSelectedUnits().PerformCommand(Command.Move, GetWorldMousePoint());
            }
        }
    }

    private Vector3 GetWorldMousePoint()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f, groundLayer))
        {
            return hit.point;
        }

        Debug.LogWarning("Did not clicked on the ground layer.");
        return Vector3.zero;
    }

    private Command GetCurrentCommandAccordingToInput()
    {
        foreach (var c in allCommands)
        {
            if (!c.Equals(Command.None) && Input.GetButtonDown(c.ToString()))
            {
                return c;
            }
        }
        return Command.None;
    }

    public void SetSelectionEnabled(bool enabled)
    {
        if (selectRectangle != null)
        {
            selectRectangle.enabled = enabled;
        }
    }
}

public enum Command
{
    Attack, Hold, Stop, Move, Skill0, Skill1, Skill2, Skill3, None
}


