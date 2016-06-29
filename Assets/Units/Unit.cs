using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
	public int hp;
	public int mp;
	public float walkSpeed;
	public float runSpeed;
	public float attackSpeed;
	public float radius;

	public ParticleSystemPlayer Skill0;
	public ParticleSystemPlayer Skill1;
	public ParticleSystemPlayer Skill2;
	public ParticleSystemPlayer Skill3;

	protected NavMeshAgent agent;
	protected Animator animator;
	protected bool isHold = false;
	protected bool isRunning = false;

	private Vector3 lastPos;
	private float timeUntilCheck = 1;

	private readonly float aiCheckingInterval = 0.1f;
	private readonly float aiStoppingConstant = 0.5f;
	private readonly float aiAnimationIgnoreSpeed = 0.5f;
	private readonly float aiDistanceCloseToDestination = 5f;

	protected virtual void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();

		lastPos = transform.position;
	}

	void FixedUpdate()
	{
		agent.speed = (isRunning) ? runSpeed : walkSpeed;
		agent.radius = radius;

		StopIfAgentIsStuck();
		SetAnimatorSpeedIfAgentIsMovingProperly();
	}

	private void StopIfAgentIsStuck()
	{
		timeUntilCheck -= Time.fixedDeltaTime;
		if (timeUntilCheck < 0)
		{
			if (Vector3.Distance(lastPos, transform.position) < aiStoppingConstant * agent.speed * aiCheckingInterval && agent.remainingDistance <= aiDistanceCloseToDestination)
			{
				agent.destination = transform.position;
			}
			timeUntilCheck = aiCheckingInterval;
			lastPos = transform.position;
		}
	}

	private void SetAnimatorSpeedIfAgentIsMovingProperly()
	{
		if (animator != null)
		{
			if (agent.velocity.magnitude > aiAnimationIgnoreSpeed && agent.remainingDistance > 0.1f)
			{
				animator.SetFloat("Speed", agent.velocity.magnitude);
			}
			else
			{
				animator.SetFloat("Speed", 0);
			}
		}
	}

	public virtual bool IsWaypointNecessary(Command command)
	{
		bool ret = false;
		switch (command)
		{
			case Command.Move:
			case Command.Attack:
				ret = true;
				break;
		}
		return ret;
	}

	public virtual void PerformCommand(Command command, Vector3 target = default(Vector3), bool onSpecificUnit = false, Unit unit = null)
	{
		switch (command)
		{
			case Command.Hold:
				isHold = true;
				agent.ResetPath();
				break;
			case Command.Stop:
				agent.ResetPath();
				break;
			case Command.Move:
				isHold = false;
				timeUntilCheck = aiCheckingInterval * 5;
				agent.SetDestination(target);
				break;
			case Command.Attack:
				isHold = false;
				timeUntilCheck = aiCheckingInterval * 5;
				agent.SetDestination(target);
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

public enum Command
{
	Skill0, Skill1, Skill2, Skill3, Attack, Hold, Stop, Move, None
}
