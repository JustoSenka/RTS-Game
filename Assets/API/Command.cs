using UnityEngine;
using System.Collections;

[System.Serializable]
public struct Command {

    public CommandType type;
    public Vector3 pos;
    public Unit unit;

    public Command(CommandType type, Vector3 pos = default(Vector3), Unit unit = null)
    {
        this.type = type;
        this.pos = pos;
        this.unit = unit;
    }

    public Command(Command command)
    {
        type = command.type;
        pos = command.pos;
        unit = command.unit;
    }
}

public enum CommandType
{
    Skill0, Skill1, Skill2, Skill3, Attack, Hold, Stop, Move, Busy, None
}