using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Unit))]
public class AI : MonoBehaviourSlowUpdates
{
    private Unit unit;
    private NavMeshAgent agent;
    private Command command;

    private bool isAttacking = false;

    private Vector3 lastPos;
    private readonly float aiCheckingInterval = 0.1f;
    private readonly float aiStoppingConstant = 0.5f;
    private readonly float aiDistanceCloseToDestination = 5f;
    private readonly float aiAttackRangeInaccuracy = 0.5f;

    void Start()
    {
        unit = GetComponent<Unit>();
        agent = GetComponent<NavMeshAgent>();

        lastPos = transform.position;
    }

    protected override void BeforeUpdate()
    {
        command = unit.GetCommand();

        CheckIfAgentFinishedCommand();
        CheckIfTargetedEnemyStillAlive();
        CheckIfEnemyStillInRangeWhenAttacking();
    }

    protected override void AfterUpdate()
    {
        unit.SetAttacking(isAttacking);
        unit.SetCommand(command);
    }

    private void CheckIfAgentFinishedCommand()
    {
        if (command.type.Equals(CommandType.Move) || command.type.Equals(CommandType.Attack))
        {
            if (Vector3.Distance(command.pos, transform.position) <= agent.stoppingDistance)
            {
                if (command.type.Equals(CommandType.Attack) && command.unitToAttack != null)
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
                    AttackUnit(command.unitToAttack, command.strictAttack);
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
        Debug.Log("Stop");

        isAttacking = false;
        agent.stoppingDistance = 0;
        agent.destination = unit.pos;
        command = new Command(CommandType.None);
    }

    private void AttackUnit(Unit unitToAttack, bool strictAttack = false)
    {
        if (!unit.IsHold())
        {
            isAttacking = false;
            command.type = CommandType.Attack;
            command.unitToAttack = unitToAttack;
            command.pos = unitToAttack.pos;
            command.strictAttack = strictAttack;
            agent.stoppingDistance = GetAttackRangeOnUnit(unitToAttack);
        }
        else if (Vector3.Distance(unitToAttack.pos, transform.position) <= GetAttackRangeOnUnit(unitToAttack))
        {
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

    // ------- AI UPDATE START SLOW -----------------------------------------------------------------------------------------------------------

    protected override void SlowUpdate()
    {
        StopIfAgentIsStuck();
        //CheckAllEnemyNearby();
        FollowTargetedUnit();
    }

    private void StopIfAgentIsStuck()
    {
        if (Vector3.Distance(lastPos, transform.position) < aiStoppingConstant * agent.speed * aiCheckingInterval
            && agent.remainingDistance <= aiDistanceCloseToDestination &&
            (command.type.Equals(CommandType.Move) ||
            command.type.Equals(CommandType.Attack) && command.unitToAttack == null))
        {
            Debug.Log("Stuck");
            agent.destination = transform.position;
            StopAgentFromDoingCurrentCommand();
        }
        lastPos = transform.position;
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

        if (closestUnit != null)
        {
            AttackUnit(closestUnit);
        }
    }

    private void FollowTargetedUnit()
    {
        if (command.type.Equals(CommandType.Attack) && command.unitToAttack != null && !command.unitToAttack.IsDead())
        {
            agent.destination = command.unitToAttack.pos;
            command.pos = command.unitToAttack.pos;
        }
    }

    // ------- AI UPDATE END -----------------------------------------------------------------------------------------------------------
    // ------- AI UPDATE START EVEN SLOWER ---------------------------------------------------------------------------------------------

    protected override void SlowerUpdate()
    {
        CheckAllEnemyNearby();
    }

}