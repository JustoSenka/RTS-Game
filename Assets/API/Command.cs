using UnityEngine;
using System.Collections;

[System.Serializable]
public struct Command {

	public static Command None = new Command(CommandType.None);

    public CommandType type;
    public Vector3 pos;
    public Unit unitToAttack;
    public bool strictAttack;

    public Command(CommandType type, Vector3 pos = default(Vector3), Unit unitToAttack = null, bool strictAttack = false)
    {
        this.type = type;
        this.pos = pos;
        this.unitToAttack = unitToAttack;
        this.strictAttack = strictAttack;
    }

	public bool IsNone()
	{
		return type.Equals(CommandType.None);
	}

	public bool IsSkill()
	{
		return type.GetHashCode().IsSkill();
	}

	public bool IsAttack()
	{
		return type.Equals(CommandType.Attack);
	}

	public bool IsMove()
	{
		return type.Equals(CommandType.Move);
	}
}

public enum CommandType
{
    Skill0, Skill1, Skill2, Skill3, Attack, Hold, Stop, Move, Busy, None
}