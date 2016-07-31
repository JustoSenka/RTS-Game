using UnityEngine;
using System.Collections;

[System.Serializable]
public class Skill
{
	[Header("General:")]
	public string name;

	[Header("Modules:")]
	public Main main;
	public Animator animator;
	public Offensive offensive;
	public Buff buff;
	public Summon summon;
	public Movement movement;
	public Particles particles;

	[System.Serializable]
	public struct Main
	{
		public Texture tex;
		public bool requirePath;
		public bool mustTargetUnit;
		public float range;
		public float manaCost;
		public float cooldown;
	}

	[System.Serializable]
	public struct Animator
	{
		public bool enabled;
		public string stateName;
		public float duration;
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
	public struct Summon
	{
		public bool enabled;
		public GameObject unitToSummon;
		public int numOfUnits;
		public float lifetime;
		public float delay;
	}

	[System.Serializable]
	public struct Movement
	{
		public bool enabled;
		public bool teleport;
		public float lerpSpeed;
		public float delay;
	}

	[System.Serializable]
	public struct Particles
	{
		public bool enabled;
		public float duration;
		public Vector3 position;
		public float delay;
	}

	private Skill() { }
}

public enum BuffType
{
	[FieldName("damage")] Damage,
	[FieldName("defense")] Defense,
	[FieldName("walkSpeed")] WalkSpeed,
	[FieldName("attackSpeed")] AttackSpeed,
	[FieldName("attackRange")] AttackRange,
	[FieldName("hp")] Heal,
}
