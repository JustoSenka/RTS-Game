using UnityEngine;
using System.Collections;

public class UnitHero : Unit
{
	protected override void PerformSkill(Command command)
	{
		var hash = command.type.GetHashCode();
		var skill = skills[hash];
		if (hash >= 0 && hash <= 3 && cooldowns[hash % cooldowns.Length] <= 0)
		{
			if (mp >= skill.main.manaCost)
			{
				mp -= skill.main.manaCost;
				cooldowns[hash] = skill.main.cooldown;
				SkillReqsMet(hash, skill, command);
			}
		}
	}

	private void SkillReqsMet(int index, Skill skill, Command command)
	{
		isRunning = true;
		skillParticleRef[index].Play();
		cooldowns[0] = skills[0].main.cooldown;

		this.RunAfter(skills[0].buff.duration, () =>
		{
			isRunning = false;
			skillParticleRef[index].Stop();
		});
	}
}
