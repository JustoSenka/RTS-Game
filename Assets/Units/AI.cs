using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Unit))]
public class AI : MonoBehaviour
{
    private Unit unit;
    private NavMeshAgent agent;
    private Command command;

    private bool isAttacking = false;

    private Vector3 lastPos;
    private float aiTimeUntilCheck = 1;

    private readonly float aiCheckingInterval = 0.1f;
    private readonly float aiStoppingConstant = 0.5f;
    //private readonly float aiAnimationIgnoreSpeed = 0.5f;
    private readonly float aiDistanceCloseToDestination = 5f;
    private readonly float aiAttackRangeInaccuracy = 0.5f;

    void Start()
    {
        unit = GetComponent<Unit>();
        agent = GetComponent<NavMeshAgent>();

        lastPos = transform.position;
    }

    void Update()
    {
        command = unit.GetCommand();

        CheckIfAgentFinishedCommand();
        CheckIfTargetedEnemyStillAlive();
        CheckIfEnemyStillInRangeWhenAttacking();

        AiUpdate();

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
            transform.LookAt(new Vector3(command.unitToAttack.transform.position.x, transform.position.y, command.unitToAttack.transform.position.z));
            if (Vector3.Distance(command.unitToAttack.transform.position, transform.position) > GetAttackRangeOnUnit(command.unitToAttack) + aiAttackRangeInaccuracy)
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
        command = new Command(CommandType.None);
    }

    private void AttackUnit(Unit unitToAttack, bool strictAttack = false)
    {
        if (!unit.IsHold())
        {
            isAttacking = false;
            command.type = CommandType.Attack;
            command.unitToAttack = unitToAttack;
            command.pos = unitToAttack.transform.position;
            command.strictAttack = strictAttack;
            agent.stoppingDistance = GetAttackRangeOnUnit(unitToAttack);
        }
        else if (Vector3.Distance(unitToAttack.transform.position, transform.position) <= GetAttackRangeOnUnit(unitToAttack))
        {
            command.type = CommandType.Busy;
            command.unitToAttack = unitToAttack;
            command.pos = unitToAttack.transform.position;
            isAttacking = true;
        }
    }

    private float GetAttackRangeOnUnit(Unit unitToAttack)
    {
        return unit.attackRange + unit.radius + unitToAttack.radius;
    }

    // ------- AI UPDATE START -----------------------------------------------------------------------------------------------------------

    private void AiUpdate()
    {
        aiTimeUntilCheck -= Time.deltaTime;
        if (aiTimeUntilCheck < 0)
        {
            StopIfAgentIsStuck();
            CheckAllEnemyNearby();
            FollowTargetedUnit();


            aiTimeUntilCheck = aiCheckingInterval;
        }
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
        var myPos = transform.position;
        var closestDistance = float.MaxValue;
        var allUnits = Data.GetInstance().GetAllUnits();
        foreach (var enemyUnit in allUnits)
        {
            var distance = Vector3.Distance(enemyUnit.transform.position, myPos);
            if (distance <= unit.sight)
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
        if (command.type.Equals(CommandType.Attack) && command.unitToAttack != null)
        {
            agent.destination = command.unitToAttack.transform.position;
            command.pos = command.unitToAttack.transform.position;
        }
    }

    // ------- AI UPDATE END -----------------------------------------------------------------------------------------------------------

    public void setAiTimeUntilCheck(float time)
    {
        aiTimeUntilCheck = aiCheckingInterval * time;
    }
}