using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class Unit : MonoBehaviour
{
    public Team team = Team.T1;
	public int tier;
    [Space(5)]

    [Header("Unit Stats:")]
	public float maxHp;
	public float hp;
	public float hpRegen;
	public float maxMp;
	public float mp;
	public float mpRegen;

	[Space(5)]
	public float walkSpeed;
    public float runSpeed;
    public float attackSpeed;
    public float attackRange;
    public float damage;
    public float defense;

    [Range(0, 100)]
    public float sight;
    [Space(5)]

	[Header("Physic Parameters:")]
	[Range(0.1f, 5)]
	public float radius;
	[Range(0.1f, 5)]
	public float height;
	[Space(5)]

	[Header("Skill Particles References:")]
	public ParticleSystemPlayer[] skillParticleRef = new ParticleSystemPlayer[4];
	[Space(5)]

	[Header("Skill instruction ref index:")]
	public int[] skillIndex = new int[4];
	[Space(5)]

	protected AI ai;
	protected UnityEngine.AI.NavMeshAgent agent;
    protected UnityEngine.AI.NavMeshObstacle obstacle;
    protected Animator animator;
    protected bool isDead = false;
    protected bool isHold = false;
    protected bool isRunning = false;
    protected bool isAttacking = false;

	//[NonSerialized]
	public Command command;

	[NonSerialized] public float[] cooldowns = new float[4];
	[NonSerialized] public Skill[] skills = new Skill[4];
	/*[NonSerialized]*/ public Command commandPending;
	[NonSerialized] public Vector3 pos;
    
	private GameObject DeathCallback;

	protected virtual void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        obstacle = GetComponent<UnityEngine.AI.NavMeshObstacle>();
        animator = GetComponent<Animator>();
        ai = GetComponent<AI>();

        DeathCallback = GameObject.FindGameObjectWithTag("GameController");
        command = Command.None;
		commandPending = Command.None;

		for (int i = 0; i < 4; i++)
		{
			skills[i] = Data.GetInstance().skills[skillIndex[i]];
		}

		if (agent)
		{
			agent.radius = radius;
			agent.height = height;
		}

        if (obstacle)
        {
            obstacle.shape = UnityEngine.AI.NavMeshObstacleShape.Capsule;
            obstacle.center = new Vector3(0, (height / 2) - (radius / 2), 0);
            obstacle.radius = radius - 0.2f;
            obstacle.height = height;
        }
    }

    protected virtual void Update()
    {
        float delta = Time.deltaTime;
		for (int i = 0; i < cooldowns.Length; i++)
		{
			cooldowns[i] -= delta;
		}

		hp += hpRegen * delta;
		mp += mpRegen * delta;
		if (hp > maxHp) hp = maxHp;
		if (mp > maxMp) mp = maxMp;
	}

    protected virtual void FixedUpdate()
    {
        pos = transform.position;

        agent.speed = (isRunning) ? runSpeed : walkSpeed;
        SetAnimatorSpeedIfAgentIsMovingProperly();
        animator.SetBool("Attack", isAttacking);
    }

    private void SetAnimatorSpeedIfAgentIsMovingProperly()
    {
        if (!command.type.Equals(CommandType.None) && !command.type.Equals(CommandType.Hold) && !command.type.Equals(CommandType.Busy)
            && agent.velocity.magnitude > 0.1)
        {
            animator.SetFloat("Speed", agent.speed);
            animator.SetBool("Running", isRunning);
        }
        else
        {
            animator.SetFloat("Speed", 0);
            animator.SetBool("Running", false);
        }
            
    }

    public bool DealDamage(float damageIncome)
    {
        hp -= (damageIncome > defense) ? damageIncome - defense : 0;
        if (hp <= 0)
        {
            if (DeathCallback)
            {
                DeathCallback.SendMessage("DeathCallback", gameObject);
            }
            animator.SetBool("Death", isDead = true);
            agent.enabled = false;
            if (ai != null) ai.enabled = false;
        }
        return hp <= 0;
    }

	// Callback from animator
    public void AttackAnimationCallback()
    {
        if (command.unitToAttack != null)
        {
            command.unitToAttack.DealDamage(damage);
        }
		//@TODO: give xp
	}

	// Callback from projectile particle
	public void ProjectileParticleCallback(Unit unitHit)
	{
		//@TODO: give xp
	}

	public void Destroy()
    {
        Data.GetInstance().RemoveUnit(this);
        Destroy(gameObject);
    }

    public bool IsWaypointNecessary(Command command)
    {
		int hash = command.type.GetHashCode();
		if (command.type.Equals(CommandType.Move) || command.type.Equals(CommandType.Attack))
		{
			return true;
		}
		else if (hash.IsSkill() && skills[hash % 4] != null)
		{
			return skills[hash].main.path.requireSecondClick();
		}
		else
		{
			return false;
		}
	}

	public bool IsSkillUsedOnUnit(Command command)
	{
		Skill skill = GetSkill(command);
		return (skill == null) ? false : skill.main.path.Equals(Path.OnUnit);
	}

	public Skill GetSkill(Command command)
	{
		int hash = command.type.GetHashCode();
		if (hash.IsSkill() && skills[hash % 4] != null)
		{
			return skills[hash];
		}
		else
		{
			return null;
		}
	}

	// Virtual methods: ------------------------------------------------------------------------------------

	public virtual void PerformCommand(Command command, bool resetPendingCommand = true)
    {
        if (isDead)
            return;

		if (resetPendingCommand)
			commandPending = Command.None;

		switch (command.type)
        {
            case CommandType.Hold:
                this.command = command;
                isHold = true;
                if (agent.enabled) agent.ResetPath();
                break;
            case CommandType.Stop:
                this.command = new Command(CommandType.None);
                isHold = false;
                if (agent.enabled) agent.ResetPath();
                break;
            case CommandType.Move:
                this.command = command;
                isHold = false;
                if (agent.enabled) agent.SetDestination(command.pos);
                if (ai) StartCoroutine(ai.DirectCommandOnUnit(this.command));
                break;
            case CommandType.Attack:
                this.command = command;
                isHold = false;
                if (agent.enabled) agent.SetDestination(command.pos);
                if (command.strictAttack && command.unitToAttack == this)
                {
                    this.command.strictAttack = false;
                    this.command.unitToAttack = null;
                }
                if (ai) StartCoroutine(ai.DirectCommandOnUnit(this.command));
                break;
			default:
				PerformSkill(command);
				break;
        }
	}

	// This should be overriden by child class
	protected virtual void PerformSkill(Command command){ }
	public virtual void PerformPendingSkill() { }

	public Command GetCommand() { return command; }
    public void SetCommand(Command command) { this.command = command; }
    public void SetAttacking(bool isAttacking) { this.isAttacking = isAttacking; }
    public bool IsHold() { return isHold; }
    public bool IsDead() { return isDead; }
}

public enum Team
{
    T1, T2, T3, T4, T5, T6, T7, T8
}

public class UnitComparer : IComparer<Unit>
{
	public int Compare(Unit x, Unit y)
	{
		if (x == y)
		{
			return 0;
		}
		else if (x.tier > y.tier)
		{
			return 1;
		}
		else
		{
			return -1;
		}
	}
}
