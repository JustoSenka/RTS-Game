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

	public void DeathCallback(GameObject unitGO)
	{
		// Disable skill range projector as soon as unit died
		var projector = unitGO.GetComponent<RangeProjector>();
		if (projector)
		{
			projector.Disable();
		}
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

				var unitClickedOn = Common.GetObjectUnderMouse().GetComponent<Unit>();
				var topUnit = selectedUnits[0];
				var commandHash = currentCommand.type.GetHashCode();

				// Skill on unit
				if (commandHash.IsSkill() && topUnit.skills[commandHash % 4].main.path.Equals(Path.OnUnit))
				{
					if (unitClickedOn)
					{
						selectedUnits.PerformCommand(new Command(currentCommand.type, unitClickedOn.transform.position, unitClickedOn, true), true);
						moveCross.ShowAt(unitClickedOn.transform.position, true);
					}
					else
					{
						Debug.Log("This skill must target unit");
						// play sound or do smth..
					}
				}
				else 
				{
					// Attack on unit
					if (unitClickedOn && currentCommand.type.Equals(CommandType.Attack))
					{
						selectedUnits.PerformCommand(new Command(currentCommand.type, unitClickedOn.transform.position, unitClickedOn, true), true);
						moveCross.ShowAt(unitClickedOn.transform.position, true);
					}
					// Skill on ground (maybe unit, but not strict), or move
					else
					{
						var commandPos = (unitClickedOn) ? unitClickedOn.pos : Common.GetWorldMousePoint(groundLayer);
						moveCross.ShowAt(commandPos, !currentCommand.type.Equals(CommandType.Move));
						currentCommand.pos = commandPos;
						selectedUnits.PerformCommand(currentCommand, true);
					}
				}

				// Disable all range projectors
				selectRectangle.GetSelectedUnits().ForEach((u) => u.GetComponent<RangeProjector>().Disable());
			}
            else if (skillPhase && Input.GetMouseButtonDown(MouseButton.Right.GetHashCode()))
            {
                SetSelectionEnabled(true);
                currentCommand = new Command(CommandType.None);
                skillPhase = false;

				// Disable all range projectors
				selectRectangle.GetSelectedUnits().ForEach((u) => u.GetComponent<RangeProjector>().Disable());
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
                        selectedUnits.PerformCommand(new Command(CommandType.Attack, unitClickedOn.transform.position, unitClickedOn, true), true);
						moveCross.ShowAt(unitClickedOn.transform.position, true);
					}
					// Right click on ground or anywhere else
					else
					{
						var mousePos = Common.GetWorldMousePoint(groundLayer);
						moveCross.ShowAt(mousePos);
                        selectedUnits.PerformCommand(new Command(CommandType.Move, mousePos), true);
					}
				}
            }
        }
		// Selected unit count less than one
		else
		{
			SetSelectionEnabled(true);
			currentCommand = new Command(CommandType.None);
		}
    }

    public void PerformCommand(Command commandToPerform)
    {
		var topUnit = selectRectangle.GetSelectedUnits()[0];

		// If skill not ready or no mana, do nothing
		var hash = commandToPerform.type.GetHashCode();
		if (hash.IsSkill() && (topUnit.cooldowns[hash % 4] > 0 || topUnit.mp < topUnit.skills[hash % 4].main.manaCost))
			return;

		// Set command and skill phase accodingly
		currentCommand = commandToPerform;
		skillPhase = !currentCommand.type.Equals(CommandType.None);
		SetSelectionEnabled(!skillPhase);

		// If waypoint not necessary - perform
		if (!currentCommand.type.Equals(CommandType.None) && !topUnit.IsWaypointNecessary(currentCommand))
        {
            selectRectangle.GetSelectedUnits().PerformCommand(currentCommand, true);
            skillPhase = false;
        }

		// Enable range projector on top units
		if (skillPhase && hash.IsSkill() && topUnit.skills[hash % 4].main.path.isRangeUsed())
		{
			selectRectangle.GetSelectedUnits().ForEachOnHighestTier((u) => u.GetComponent<RangeProjector>().Enable(u.skills[hash].main.range));
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
        if (selectRectangle && selectRectangle.enabled != enabled)
        {
            selectRectangle.enabled = enabled;
        }
    }

    public bool IsInSkillPhase()
    {
        return skillPhase;
    }

	public Command GetCurrentCommand()
	{
		return currentCommand;
	}

    private enum MouseButton { Left, Right, Midle }
}


