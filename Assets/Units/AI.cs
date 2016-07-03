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
    private readonly float aiAnimationIgnoreSpeed = 0.5f;
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
        agent.stoppingDistance = unit.attackRange;
    }


    // ------- AI UPDATE START -----------------------------------------------------------------------------------------------------------

    private void AiUpdate()
    {
        aiTimeUntilCheck -= Time.deltaTime;
        if (aiTimeUntilCheck < 0)
        {
            StopIfAgentIsStuck();
            CheckIfEnemyNearby();
            PerformLongTermCommand();

            aiTimeUntilCheck = aiCheckingInterval;
        }
    }

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
        if (command.type.Equals(CommandType.Move) || 
            command.type.Equals(CommandType.Busy))
            return;

        Unit closestUnit = null;
        float closestDistance = float.MaxValue;
        var allUnits = FindObjectsOfType<Unit>();
        foreach (var enemyUnit in allUnits)
        {
            if (!enemyUnit.team.Equals(unit.team) && Vector3.Distance(transform.position, enemyUnit.transform.position) <= unit.sight)
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
            if (Vector3.Distance(command.unit.transform.position, transform.position) > unit.attackRange + aiAttackRangeInaccuracy)
            {
                AttackSpecificUnit(command.unit);
            }
        }
    }

    // ------- AI UPDATE END -----------------------------------------------------------------------------------------------------------

    public void setAiTimeUntilCheck(float time)
    {
        aiTimeUntilCheck = aiCheckingInterval * time;
    }
}