using UnityEngine;
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
			if (!skill.main.path.isRangeUsed() || Common.GetRawDistance2D(pos, command.pos) < skill.main.range * skill.main.range)
			{
				mp -= skill.main.manaCost;
				cooldowns[hash] = skill.main.cooldown;
				SkillReqsMet(hash, skill, command);
			}
			// Too far in range, move closer
			else
			{
				commandPending = command;
				PerformCommand(new Command(CommandType.Move, command.pos), false);
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
			this.RunAfter(skill.offensive.delay, () =>
			{

			});
		}

		if (skill.buff.enabled)
		{
			this.RunAfter(skill.buff.delay, () =>
			{
				Unit targetedUnit = (skill.main.path.Equals(Path.OnUnit)) ? command.unitToAttack : this;

				targetedUnit.IncreaseFieldValueBy(skill.buff.buffType, skill.buff.increaseValue);
				if (!skill.buff.buffType.Equals(BuffType.Heal))
					targetedUnit.RunAfter(skill.buff.duration, () => targetedUnit.DecreaseFieldValueBy(skill.buff.buffType, skill.buff.increaseValue));
			});
		}

		if (skill.summon.enabled)
		{
			this.RunAfter(skill.summon.delay, () =>
			{
				for (int i = 0; i < skill.summon.numOfUnits; i++)
				{
					Unit minion = Instantiate(skill.summon.unitToSummon).GetComponent<Unit>();
					minion.transform.position = Common.GetRandomVector(0.5f) + this.pos;
					minion.transform.SetParent(GameObject.FindGameObjectWithTag("Generated").transform);
					minion.team = this.team;
					minion.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;

					// Create units at position
					if (skill.main.path.Equals(Path.Range))
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
			});
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
			foreach (var particle in skill.particles.array)
			{
				// Should pass particle as argument, because it changes afterwards in foreach operation
				this.RunAfter(particle.delay, particle, (p) =>
				{
					var ps = Instantiate(Particles.Instance[p.id]);

					ps.transform.SetParent(GetPsUnitAttachment(p, skill, command));
					ps.transform.position = GetPsStartPosition(p, skill, command);
					ps.transform.position += p.startPosOffset;
					ps.transform.localRotation = Quaternion.Euler(-90, 0, 0);

					// Everything for projectiles
					if (p.particleType.Equals(ParticleType.Projectile))
					{
						Vector3 posToLookAt = (command.unitToAttack) ? command.unitToAttack.pos : command.pos;
						posToLookAt += p.targetPosOffset;
						ps.transform.LookAt(posToLookAt);

						var pp = ps.GetComponent<ParticleProjectile>();
						if (!pp) { Debug.LogWarning("Particle must have ParticleProjectile component attached"); return; }

						// General stats
						pp.projectileLauncher = gameObject;
						pp.friendlyFire = p.friendlyFire;
						pp.oneShot = p.oneShot;
						pp.team = team;
						pp.Launch(posToLookAt, p.projecticleSpeed);

						// Ofensive stats
						pp.damage = (skill.offensive.enabled) ? skill.offensive.damage : 0;
						pp.areaOfEffect = (skill.offensive.enabled) ? skill.offensive.areaOfEffect : 0;

						// Play and destroy afterwards
						pp.RunAfter(p.duration, () => pp.DestroyParticles());
						pp.Play(false);
					}
					// Simple particle effect, buff or any
					else
					{
						float duration = (p.oneShot) ? ps.duration : p.duration;
						Particles.Instance.RunAfter(duration, () => { if (ps) Destroy(ps.gameObject); });

						ps.SetLoop(!p.oneShot);
						ps.Play(true);
					}
				});
			}
		}

		commandPending = Command.None;
	}

	private Transform GetPsUnitAttachment(Skill.SingleParticle p, Skill skill, Command command)
	{
		switch (p.unitAttachment)
		{
			case UnitAttachment.Self:
				return transform;
			case UnitAttachment.Enemy:
				if (!command.unitToAttack)
				{
					Debug.LogWarning("Skill must target unit, for particles to be attached on them");
					return GameObject.FindGameObjectWithTag("Generated").transform;
				}
				return command.unitToAttack.transform;
			case UnitAttachment.World:
				return GameObject.FindGameObjectWithTag("Generated").transform;
			default:
				return null;
		}
	}

	private Vector3 GetPsStartPosition(Skill.SingleParticle p, Skill skill, Command command)
	{
		switch (p.startPosition)
		{
			case StartPosition.Self:
				return pos;
			case StartPosition.Enemy:
				if (!command.unitToAttack)
				{
					Debug.LogWarning("Skill must target unit, for particles to show up on them");
					return command.pos;
				}
				return command.unitToAttack.pos;
			case StartPosition.Mouse:
				return command.pos;
			default:
				return pos;
		}
	}

}
