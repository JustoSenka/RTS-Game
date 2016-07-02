using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitSpider : Unit
{
    protected override void Start()
    {
        base.Start();
    }

    public override bool IsWaypointNecessary(Command command)
    {
        bool ret = false;

        if (base.IsWaypointNecessary(command)) return true;

        switch (command.type)
        {
            case CommandType.Skill2:
            case CommandType.Skill3:
                ret = true;
                break;
        }
        return ret;
    }

    public override void PerformCommand(Command command)
    {
        base.PerformCommand(command);

        switch (command.type)
        {
            case CommandType.Skill0:
                isRunning = true;
                Skill0.Play();

                this.RunAfter(6f, () =>
                {
                    isRunning = false;
                    Skill0.Stop();
                });

                break;
            case CommandType.Skill1:

                break;
            case CommandType.Skill2:

                break;
            case CommandType.Skill3:

                break;
        }
    }
}
