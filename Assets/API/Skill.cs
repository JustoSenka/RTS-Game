using UnityEngine;
using System.Collections;

[System.Serializable]
public class Skill
{
	[Header("General:")]
	public string name;

	[Header("Modules:")]
	public Main main;
	public Offensive offensive;
	public Buff buff;
	public Movement movement;

	[System.Serializable]
	public struct Main
	{
		public Texture tex;
		public SkillType skillType;
		public bool requirePath;
		public bool mustTargetUnit;
		public float range;
		public float manaCost;
		public float cooldown;
	}

	[System.Serializable]
	public struct Offensive
	{
		public bool enabled;
		public float damage;
		public float areaOfEffect;
	}

	[System.Serializable]
	public struct Buff
	{
		public bool enabled;
		public BuffType buffType;
		public float increaseValue;
		public float duration;
	}

	[System.Serializable]
	public struct Movement
	{
		public bool enabled;
		public bool teleport;
		public float lerpSpeed;
	}

	private Skill() { }
}


public enum SkillType
{
	Melee, Magic, Misisile, Buff, Movement, Passive
}

public enum BuffType
{
	Damage, Armor, Speed, Heal, Steal
}
