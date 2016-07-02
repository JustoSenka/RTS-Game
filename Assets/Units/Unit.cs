using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Unit : MonoBehaviour
{
    [Header("Unit Stats:")]
    public int hp;
    public int mp;
    public float walkSpeed;
    public float runSpeed;
    public float attackSpeed;
    public float attackRange;
    public float damage;
    public float defense;
    [Range(0.1f, 3)]
    public float radius;
    [Range(0, 100)]
    public float sight;
    [Space(5)]

    [Header("Skill Object References:")]
	public ParticleSystemPlayer Skill0;
	public ParticleSystemPlayer Skill1;
	public ParticleSystemPlayer Skill2;
	public ParticleSystemPlayer Skill3;
    [Space(5)]

    [Header("Team:")]
    public Team team = Team.T1;

    protected NavMeshAgent agent;
	protected Animator animator;
	protected bool isHold = false;
	protected bool isRunning = false;
    protected bool isAttacking = false;

    public Command command = new Command(CommandType.None);

    // AI
	private Vector3 lastPos;
	private float aiTimeUntilCheck = 1;
    
	private readonly float aiCheckingInterval = 0.1f;
	private readonly float aiStoppingConstant = 0.5f;
	private readonly float aiAnimationIgnoreSpeed = 0.5f;
	private readonly float aiDistanceCloseToDestination = 5f;
    private readonly float aiAttackRangeInaccuracy = 0.5f;

    protected virtual void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();

        command.type = CommandType.None;
        agent.radius = radius;
        lastPos = transform.position;
	}

    protected virtual void FixedUpdate()
	{
		agent.speed = (isRunning) ? runSpeed : walkSpeed;

        CheckIfAgentFinishedCommand();

        AiUpdate();

		SetAnimatorSpeedIfAgentIsMovingProperly();

        animator.SetBool("Attack", isAttacking);
    }

    private void CheckIfAgentFinishedCommand()
    {
        if (command.type.Equals(CommandType.Move) || command.type.Equals(CommandType.Attack))
        {
            if (Vector3.Distance(command.pos, transform.position) <= agent.stoppingDistance)
            {
                if (command.type.Equals(CommandType.Attack) && command.unit != null)
                {
                    Debug.Log("Finished, now attacking");
                    command.type = CommandType.Busy;
                    isAttacking = true;
                }
                else
                {
                    Debug.Log("Finished and will do Stop");
                    StopAgentFromDoingCurrentCommand();
                }
            }
        }
    }

    private void AiUpdate()
    {
        aiTimeUntilCheck -= Time.fixedDeltaTime;
        if (aiTimeUntilCheck < 0)
        {
            StopIfAgentIsStuck();
            CheckIfEnemyNearby();
            PerformLongTermCommand();

            aiTimeUntilCheck = aiCheckingInterval;
        }
    }

    // ------- AI UPDATE START -----------------------------------------------------------------------------------------------------------

    private void StopIfAgentIsStuck()
	{
		if (Vector3.Distance(lastPos, transform.position) < aiStoppingConstant * agent.speed * aiCheckingInterval
            && agent.remainingDistance <= aiDistanceCloseToDestination && (command.type.Equals(CommandType.Move) || 
            command.type.Equals(CommandType.Attack) && command.unit == null))
        {
            Debug.Log("Stuck");
            agent.destination = transform.position;
            StopAgentFromDoingCurrentCommand();
        }
        lastPos = transform.position;
	}

    private void CheckIfEnemyNearby()
    {
        if (command.type.Equals(CommandType.Move) || /*(command.type.Equals(CommandType.Attack) && command.unit != null) ||*/
            command.type.Equals(CommandType.Busy))
            return;

        Unit closestUnit = null;
        float closestDistance = float.MaxValue;
        var allUnits = FindObjectsOfType<Unit>();
        foreach (var enemyUnit in allUnits)
        {
            if (!enemyUnit.team.Equals(team) && Vector3.Distance(transform.position, enemyUnit.transform.position) <= sight)
            {
                if (closestUnit == null || Vector3.Distance(enemyUnit.transform.position, transform.position) < closestDistance)
                {
                    closestUnit = enemyUnit;
                    closestDistance = Vector3.Distance(closestUnit.transform.position, transform.position);
                }
            }
        }

        if (closestUnit != null)
        {
            AttackSpecificUnit(closestUnit);
        }
    }

    private void PerformLongTermCommand()
    {
        // Follow specific unit if it is targeted
        if (command.type.Equals(CommandType.Attack) && command.unit != null)
        {
            agent.destination = command.unit.transform.position;
            command.pos = command.unit.transform.position;
        }
        // When attacking, check if not too far away and look at it
        if (command.type.Equals(CommandType.Busy) && isAttacking)
        {
            transform.LookAt(new Vector3(command.unit.transform.position.x, transform.position.y, command.unit.transform.position.z));
            if (Vector3.Distance(command.unit.transform.position, transform.position) > attackRange + aiAttackRangeInaccuracy)
            {
                AttackSpecificUnit(command.unit);
            }
        }
    }

    // ------- AI UPDATE END ---------------------------------------------------------------------------------------------------------------

    private void SetAnimatorSpeedIfAgentIsMovingProperly()
	{
		if (animator != null)
		{
			if (!command.type.Equals(CommandType.None) && !command.type.Equals(CommandType.Hold) && !command.type.Equals(CommandType.Busy))
			{
				animator.SetFloat("Speed", agent.velocity.magnitude);
			}
			else
			{
				animator.SetFloat("Speed", 0);
			}
		}
	}

    private void StopAgentFromDoingCurrentCommand()
    {
        if (command.type.Equals(CommandType.Busy) && isAttacking) Debug.LogWarning("What could have stopped unit from attacking ?");
        Debug.Log("Stop");

        isAttacking = false;
        agent.stoppingDistance = 0;
        command = new Command(CommandType.None);
    }

    private void AttackSpecificUnit(Unit unit)
    {
        isAttacking = false;
        command.type = CommandType.Attack;
        command.unit = unit;
        command.pos = unit.transform.position;
        agent.stoppingDistance = attackRange;
    }

    public virtual bool IsWaypointNecessary(Command command)
	{
		bool ret = false;
		switch (command.type)
		{
			case CommandType.Move:
			case CommandType.Attack:
				ret = true;
				break;
		}
		return ret;
	}

	public virtual void PerformCommand(Command command)
	{
        if (command.type.Equals(CommandType.Busy)) Debug.LogWarning("Perform Command BUSY ????");

		switch (command.type)
		{
			case CommandType.Hold:
                this.command = command;
                isHold = true;
				agent.ResetPath();
				break;
			case CommandType.Stop:
                this.command = command;
                this.command.type = CommandType.None;
                agent.ResetPath();
				break;
			case CommandType.Move:
                this.command = command;
                isHold = false;
				aiTimeUntilCheck = aiCheckingInterval * 5;
				agent.SetDestination(command.pos);
				break;
			case CommandType.Attack:
                this.command = command;
                isHold = false;
				aiTimeUntilCheck = aiCheckingInterval * 5;
				agent.SetDestination(command.pos);
				break;
		}
	}

	/*
	void OnTriggerEnter(Collider other)
	{

	}

	public void OnTriggerExit(Collider other)
	{

	}
	*/
}

public enum Team
{
    T1, T2, T3, T4, T5, T6, T7, T8
}
