using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Unit))]
public class AI : MonoBehaviourSlowUpdates
{
    private Unit unit;
    private UnityEngine.AI.NavMeshAgent agent;
    private UnityEngine.AI.NavMeshObstacle obstacle;
    private Command command;

    private bool isAttacking = false;
    private float stoppingDistance = 0;

    private Vector3 lastPos;
    private readonly float aiCheckingInterval = 0.1f;
    private readonly float aiStoppingConstant = 0.5f;
    private readonly float aiDistanceCloseToDestination = 5f;
    private readonly float aiAttackRangeInaccuracy = 0.5f;
    private readonly float aiFollowingSightMultiplier = 3f;

    void Start()
    {
        unit = GetComponent<Unit>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        obstacle = GetComponent<UnityEngine.AI.NavMeshObstacle>();

        lastPos = transform.position;
    }

    protected override void BeforeUpdate()
    {
		if (agent.enabled && obstacle.enabled)
		{
			Debug.LogError("Both agent and obstacle are enabled!");
		}

		command = unit.GetCommand();

		//DontDoStupidLongDistanceFollowingCommand();
		CheckIfUnitIsCloseToPerformSkill();
        CheckIfAgentFinishedCommand();
        CheckIfTargetedEnemyStillAlive();
        CheckIfEnemyStillInRangeWhenAttacking();
    }

	protected override void AfterUpdate()
    {
        unit.SetAttacking(isAttacking);
        unit.SetCommand(command);
    }

    private void DontDoStupidLongDistanceFollowingCommand()
    {
        // Does not work, remainingDistance is always infinity if you set destination point out of nav mesh (inside and obstacle)
        if (agent.enabled && command.type.Equals(CommandType.Attack) && command.unitToAttack != null && !command.unitToAttack.IsDead() &&
            !command.strictAttack && agent.remainingDistance > unit.sight * aiFollowingSightMultiplier && agent.remainingDistance != float.PositiveInfinity)
        {
            unit.Log(agent.remainingDistance);
            agent.destination = unit.pos;
        }
    }

	private void CheckIfUnitIsCloseToPerformSkill()
	{
		if (command.type.Equals(CommandType.Move) && !unit.commandPending.IsNone())
		{
			// If on spceific unit and it's dead, cancel everything
			Skill skill = unit.GetSkill(unit.commandPending);
			if (skill && skill.main.path.Equals(Path.OnUnit))
			{
				if (!unit.commandPending.unitToAttack || unit.commandPending.unitToAttack.IsDead())
				{
					StopAgentFromDoingCurrentCommand();
					unit.commandPending = Command.None;
					return;
				}
			}

			// Close enough, perform skill
			var distance = Vector3.Distance(unit.pos, unit.commandPending.pos);
			if (distance < skill.main.range)
			{
				StopAgentFromDoingCurrentCommand();
				unit.PerformPendingSkill();
			}
			// Follow unit
			else if (skill.main.path.Equals(Path.OnUnit))
			{
				if (agent && agent.isOnNavMesh) agent.SetDestination(unit.commandPending.unitToAttack.pos);
				unit.commandPending.pos = unit.commandPending.unitToAttack.pos;
			}
		}
	}

	private void CheckIfAgentFinishedCommand()
    {
        if (command.type.Equals(CommandType.Move) || command.type.Equals(CommandType.Attack))
        {
            var distance = Vector3.Distance(unit.pos, command.pos);
            if (distance <= stoppingDistance)
            {
				// Attack and close to enemy, stop and attack
                if (command.type.Equals(CommandType.Attack) && command.unitToAttack)
                {
                    unit.Log("Finished, now attacking");
                    command.type = CommandType.Busy;
                    isAttacking = true;
                    if (agent && agent.isOnNavMesh)
                    {
                        agent.ResetPath();
                    }
                }
				// attack or move without enemies nearby, just stop
                else
                {
                    unit.Log("Finished and will do Stop");
                    StopAgentFromDoingCurrentCommand();
                }
            }
        }

        if (!command.type.Equals(CommandType.Busy))
        {
            isAttacking = false;
        }
    }

    private void CheckIfTargetedEnemyStillAlive()
    {
        if (command.unitToAttack != null && command.unitToAttack.IsDead() &&
            (command.type.Equals(CommandType.Busy) && isAttacking || command.type.Equals(CommandType.Attack)))
        {
            isAttacking = false;
            StopAgentFromDoingCurrentCommand();
        }
    }

    private void CheckIfEnemyStillInRangeWhenAttacking()
    {
        if (command.type.Equals(CommandType.Busy) && isAttacking)
        {
            transform.LookAt(new Vector3(command.unitToAttack.pos.x, transform.position.y, command.unitToAttack.pos.z));
            if (Vector3.Distance(command.unitToAttack.pos, transform.position) > GetAttackRangeOnUnit(command.unitToAttack) + aiAttackRangeInaccuracy)
            {
                if (!unit.IsHold())
                {
                    StartCoroutine(AttackUnit(command.unitToAttack, command.strictAttack));
                }
                else
                {
                    command.type = CommandType.Hold;
                    isAttacking = false;
                }
            }
        }
    }

    // -- Update ENDS -------------------------------------------------------------------------------------------------------------

    private void StopAgentFromDoingCurrentCommand()
    {
        if (command.type.Equals(CommandType.Busy) && isAttacking) Debug.LogWarning("What could have stopped unit from attacking ?");
        unit.Log("Stop");

        isAttacking = false;
        if (agent && agent.isOnNavMesh)
        {
            stoppingDistance = 0;
            agent.ResetPath();
        }
		command = Command.None;
    }

    private IEnumerator AttackUnit(Unit unitToAttack, bool strictAttack = false)
    {
        if (!unit.IsHold())
        {
            isAttacking = false;
            command.type = CommandType.Attack;
            command.unitToAttack = unitToAttack;
            command.pos = unitToAttack.pos;
            command.strictAttack = strictAttack;
            stoppingDistance = GetAttackRangeOnUnit(unitToAttack);
            if (!agent.enabled)
            {
                obstacle.enabled = false;
                yield return null;
                agent.enabled = true;
                unit.Log("Follow him, Attack!!!");
            }
        }
        else if (Vector3.Distance(unitToAttack.pos, transform.position) <= GetAttackRangeOnUnit(unitToAttack))
        {
            unit.Log("");
            command.type = CommandType.Busy;
            command.unitToAttack = unitToAttack;
            command.pos = unitToAttack.pos;
            isAttacking = true;
        }
    }

    private float GetAttackRangeOnUnit(Unit unitToAttack)
    {
        return unit.attackRange + unit.radius + unitToAttack.radius;
    }

	/// <summary>
	/// Avoid using this function, only callback from unit
	/// </summary>
	public IEnumerator DirectCommandOnUnit(Command command)
    {
        DelaySlowUpdateBy(0.5f);
        if (command.type.Equals(CommandType.Move) || command.type.Equals(CommandType.Attack))
        {
            obstacle.enabled = false;
            yield return null; // Need to wait while nav mesh is regenerated once again
            agent.enabled = true;
            agent.SetDestination(command.pos);
        }

        if (command.type.Equals(CommandType.Attack) && command.strictAttack)
        {
            stoppingDistance = GetAttackRangeOnUnit(command.unitToAttack);
        }
    }

    // ------- AI UPDATE START SLOW -----------------------------------------------------------------------------------------------------------

    protected override void SlowUpdate()
    {
        StopIfAgentIsStuck();
        FollowTargetedUnit();
        EnableObstacleIfAgentStationary();
    }

    private void StopIfAgentIsStuck()
    {
        if (agent.enabled && Vector3.Distance(lastPos, transform.position) < aiStoppingConstant * agent.speed * aiCheckingInterval
            && agent.remainingDistance <= aiDistanceCloseToDestination &&
            (command.type.Equals(CommandType.Move) ||
            command.type.Equals(CommandType.Attack) && command.unitToAttack == null))
        {
            unit.Log("Stuck");
            StopAgentFromDoingCurrentCommand();
        }
        lastPos = transform.position;
    }

    private void FollowTargetedUnit()
    {
        if (agent.enabled && command.type.Equals(CommandType.Attack) && command.unitToAttack != null && !command.unitToAttack.IsDead())
        {
            agent.destination = command.unitToAttack.pos;
            command.pos = command.unitToAttack.pos;
        } 
    }

    private void EnableObstacleIfAgentStationary()
    {
        if (!agent.enabled)
            return;

        if (command.type.Equals(CommandType.None) || command.type.Equals(CommandType.Hold) || command.type.Equals(CommandType.Busy))
        {
            agent.enabled = false;
            obstacle.enabled = true;
        }
    }

    // ------- AI SLOW UPDATE END -----------------------------------------------------------------------------------------------------------
    // ------- AI UPDATE START EVEN SLOWER ---------------------------------------------------------------------------------------------

    protected override void SlowerUpdate()
    {
        CheckAllEnemyNearby();
    }

    private void CheckAllEnemyNearby()
    {
        if (command.type.Equals(CommandType.Move) || command.type.Equals(CommandType.Busy) || command.strictAttack)
            return;

        Unit closestUnit = null;
        var sightSquared = unit.sight * unit.sight;
        var myPos = unit.pos;
        var closestDistance = float.MaxValue;
        var allUnits = Data.GetInstance().GetAllUnits();
        foreach (var enemyUnit in allUnits)
        {
            var distance = Common.GetRawDistance2D(enemyUnit.pos, myPos);
            if (distance <= sightSquared)
            {
                if (!enemyUnit.team.Equals(unit.team))
                {
                    if ((closestUnit == null || distance < closestDistance) && !enemyUnit.IsDead())
                    {
                        closestUnit = enemyUnit;
                        closestDistance = distance;
                    }
                }
            }
        }

        if (closestUnit)
            StartCoroutine(AttackUnit(closestUnit));
    }
}