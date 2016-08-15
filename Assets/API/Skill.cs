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
		public Path path;
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
		public float delay;
	}

	[System.Serializable]
	public struct Buff
	{
		public bool enabled;
		public BuffType buffType;
		public float increaseValue;
		public float duration;
		public float delay;
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
		public SingleParticle[] array;
	}

	[System.Serializable]
	public struct SingleParticle
	{
		public int id;
		public float duration;
		public UnitAttachment unitAttachment;
		public StartPosition startPosition;
		public Vector3 startPosOffset;
		public ParticleType particleType;
		public bool oneShot;
		[Header("Valid only for Projectiles")]
		public Vector3 targetPosOffset;
		public float projecticleSpeed;
		public bool friendlyFire;
		[Space(5)]
		public float delay;
	}

	private Skill() { }

	/// <summary>
	/// Does the object exists?
	/// </summary>
	public static implicit operator bool(Skill skill)
	{
		return skill != null;
	}
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

public enum Path
{
	None, Range, OnUnit, OnlyDirection
}

public enum ParticleType
{
	Buff, Projectile
}

public enum StartPosition
{
	Self, Enemy, Mouse
}

public enum UnitAttachment
{
	Self, Enemy, World
}

public static class EnumExtensionMethods
{
	public static bool requireSecondClick(this Path p)
	{
		return !p.Equals(Path.None);
	}

	public static bool isRangeUsed(this Path p)
	{
		return p.Equals(Path.Range) || p.Equals(Path.OnUnit);
	}
}
