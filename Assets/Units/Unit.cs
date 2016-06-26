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

    public ParticleSystemPlayer Skill0;
    public ParticleSystemPlayer Skill1;
    public ParticleSystemPlayer Skill2;
    public ParticleSystemPlayer Skill3;

    protected NavMeshAgent agent;
    protected Animator animator;
    protected bool isHold = false;
    protected bool isRunning = false;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        agent.speed = (isRunning) ? runSpeed : walkSpeed;

        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
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
        Debug.Log(command);

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
                agent.SetDestination(target);
                break;
            case Command.Attack:
                isHold = false;
                agent.SetDestination(target);
                break;
        }
    }
}

public enum Command
{
    Skill0, Skill1, Skill2, Skill3, Attack, Hold, Stop, Move, None
}
