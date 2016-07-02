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

    void FixedUpdate()
    {

    }

    void Update()
    {
        if (selectRectangle.GetSelectedUnits().Count > 0)
        {
            // Checks if skill or special keys are pressed
            if (!skillPhase)
            {
                currentCommand = GetCurrentCommandAccordingToInput();
                skillPhase = !currentCommand.type.Equals(CommandType.None);
                SetSelectionEnabled(!skillPhase);

                if (!currentCommand.type.Equals(CommandType.None) && !selectRectangle.GetSelectedUnits()[0].IsWaypointNecessary(currentCommand))
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

                moveCross.ShowAt(GetWorldMousePoint(), !currentCommand.type.Equals(CommandType.Move));
                currentCommand.pos = GetWorldMousePoint();
                selectRectangle.GetSelectedUnits().PerformCommand(currentCommand);
            }
            else if (skillPhase && Input.GetMouseButtonDown(1))
            {
                SetSelectionEnabled(true);
                currentCommand = new Command(CommandType.None);
                skillPhase = false;
            }
            else if (!skillPhase && Input.GetMouseButtonDown(1))
            {
                moveCross.ShowAt(GetWorldMousePoint());
                selectRectangle.GetSelectedUnits().PerformCommand(new Command(CommandType.Move, GetWorldMousePoint()));
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
}


