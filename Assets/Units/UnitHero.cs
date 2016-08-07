using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

public class UnitHero : Unit
{
	protected override void PerformSkill(Command command)
	{
		var hash = command.type.GetHashCode();
		var skill = skills[hash];

		// This check must be here, it's crucial
		// I know, Input Control Tool and UIControler also does this check, but only for top unit!! 
		if (hash.IsSkill() && cooldowns[hash % cooldowns.Length] <= 0 && mp >= skill.main.manaCost)
		{
			// Is close in range, perform skill
			if (!skill.main.requirePath || Common.GetRawDistance2D(pos, command.pos) < skill.main.range * skill.main.range)
			{
				mp -= skill.main.manaCost;
				cooldowns[hash] = skill.main.cooldown;
				SkillReqsMet(hash, skill, command);
			}
			// Too far in range, move closer
			else
			{
				commandPending = command;
				PerformCommand(new Command(CommandType.Move, command.pos));
			}
		}
	}

	public override void PerformPendingSkill()
	{
		PerformSkill(commandPending);
		commandPending = Command.None;
	}

	private void SkillReqsMet(int index, Skill skill, Command command)
	{
		if (skill.animator.enabled)
		{
			// Might need to check if parameter exists with name given for typos (animator.parameters[0].exist?)
			animator.SetBool(skill.animator.stateName, true);
			this.RunAfter(skill.animator.duration, () => animator.SetBool(skill.animator.stateName, false));
		}

		if (skill.offensive.enabled)
		{

		}

		if (skill.buff.enabled)
		{
			// Buff on other
			if (skill.main.requirePath && skill.main.mustTargetUnit)
			{
				command.unitToAttack.IncreaseFieldValueBy(skill.buff.buffType, skill.buff.increaseValue);
				if (!skill.buff.buffType.Equals(BuffType.Heal))
					command.unitToAttack.RunAfter(skill.buff.duration, () => command.unitToAttack.DecreaseFieldValueBy(skill.buff.buffType, skill.buff.increaseValue));
			}
			// Buff on self
			else
			{
				this.IncreaseFieldValueBy(skill.buff.buffType, skill.buff.increaseValue);
				if (!skill.buff.buffType.Equals(BuffType.Heal))
					this.RunAfter(skill.buff.duration, () => this.DecreaseFieldValueBy(skill.buff.buffType, skill.buff.increaseValue));
			}

		}

		if (skill.summon.enabled)
		{
			for (int i = 0; i < skill.summon.numOfUnits; i++)
			{
				Unit minion = Instantiate(skill.summon.unitToSummon).GetComponent<Unit>();
				minion.transform.position = Common.GetRandomVector(0.5f) + this.pos;
				minion.transform.SetParent(GameObject.FindGameObjectWithTag("Generated").transform);
				minion.team = this.team;
				minion.GetComponent<NavMeshAgent>().enabled = true;

				// Create units at position
				if (skill.main.requirePath)
				{
					minion.transform.position = Common.GetRandomVector(0.5f) + command.pos;
				}
				// Follow parent command
				else
				{
					var parentUnitCommand = GetCommand();
					if (parentUnitCommand.type.Equals(CommandType.Attack) || parentUnitCommand.type.Equals(CommandType.Move))
					{
						this.RunAfterOneFrame(() => minion.PerformCommand(parentUnitCommand));
					}
				}

				//TODO: Lifetime UI (maybe use mana) (add to unit field temporary or sth that loses mana over time)
				// After mana loses health and dies. Since it needs modify Unit class, lets do it later.
				this.RunAfter(skill.summon.lifetime, () => minion.DealDamage(10000));

				Data.GetInstance().AddUnit(minion);
			}
		}

		if (skill.movement.enabled)
		{
			if (skill.movement.teleport)
			{
				// TODO: multiple Units clamp into one (sometimes)
				this.RunAfter(skill.movement.delay, () =>
				{
					transform.position = command.pos;
					PerformCommand(new Command(CommandType.Attack, Common.GetRandomVector(1) + command.pos));
				});
			}
			else
			{
				//TODO: Lerp to position
			}
		}

		if (skill.particles.enabled)
		{
			if (skill.particles.position != Vector3.zero)
				skillParticleRef[index].SetPosition(skill.particles.position);
			skillParticleRef[index].Play(skill.particles.duration);
		}

		commandPending = Command.None;
	}
}
