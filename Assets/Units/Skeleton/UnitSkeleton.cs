﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitSkeleton : Unit
{
    protected override void Start()
    {
        base.Start();
        walkSpeed = 1.2f;
        runSpeed = 4f;
    }

    public override bool IsWaypointNecessary(Command command)
    {
        bool ret = false;

        if (base.IsWaypointNecessary(command)) return true;

        switch (command)
        {
            case Command.Skill2:
            case Command.Skill3:
                ret = true;
                break;
        }
        return ret;
    }

    public override void PerformCommand(Command command, Vector3 target = default(Vector3), bool onSpecificUnit = false, Unit unit = null)
    {
        base.PerformCommand(command, target, onSpecificUnit, unit);

        switch (command)
        {
            case Command.Skill0:
                isRunning = true;
                this.RunAfter(6f, () => isRunning = false);

                break;
            case Command.Skill1:

                break;
            case Command.Skill2:

                break;
            case Command.Skill3:

                break;
        }
    }
}