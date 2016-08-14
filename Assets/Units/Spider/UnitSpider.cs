using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitSpider : UnitHero
{
    protected override void Start()
    {
        base.Start();
    }

    public void UpdateRandomAttack()
    {
        animator.SetFloat("RandomAttack", (animator.GetFloat("RandomAttack") + 1) % 3);
    }

    public override void PerformCommand(Command command, bool resetPendingCommand = true)
    {
        if (isDead)
            return;

        base.PerformCommand(command, resetPendingCommand);
    }
}
