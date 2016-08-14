using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ParticleSystem))]
public class ParticleProjectile : MonoBehaviour
{
	[System.NonSerialized]
	public Team team;
	[System.NonSerialized]
	public GameObject projectileLauncher;
	[System.NonSerialized]
	public bool friendlyFire;
	[System.NonSerialized]
	public bool oneShot;
	[System.NonSerialized]
	public float damage;
	[System.NonSerialized]
	public float areaOfEffect;

	[SerializeField]
	private bool showHitParticle;
	[SerializeField]
	private ParticleSystem hitParticle;

	private ParticleSystem ps;
	private Rigidbody rb;

	void Start()
	{
		ps = GetComponent<ParticleSystem>();
		rb = GetComponent<Rigidbody>();
	}

	public void Launch(Vector3 pos, float speed, bool lockVerticalPosition = false)
	{
		Vector3 mask = (lockVerticalPosition) ? new Vector3(1, 0, 1) : new Vector3(1, 1, 1);
		this.RunAfterOneFrame(() => rb.velocity = Vector3.Scale((pos - transform.position), mask).normalized * speed);
	}

	/// <summary>
	/// Plays all particle systems in children, ignoring the one with prefix "HIT[_*]"
	/// </summary>
	public void Play(bool playHitParticle = false)
	{
		var pss = GetComponentsInChildren<ParticleSystem>();
		foreach (var p in pss)
		{
			if (!playHitParticle && !p.gameObject.name.StartsWith("HIT_", System.StringComparison.CurrentCultureIgnoreCase))
				p.Play(false);
		}
	}

	public void DestroyParticles()
	{
		if (!ps)
			return;

		// Destroying procedure has already started, return
		var emission = ps.emission;
		if (!emission.enabled)
			return;

		emission.enabled = false;
		rb.velocity = Vector3.zero;

		// Show hit explosion and destroy whole object
		if (showHitParticle)
		{
			hitParticle.Emit(1);
			Particles.Instance.RunAfter(hitParticle.duration, () => Destroy(gameObject));
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		// Particles don't collide with each other
		if (other.gameObject.layer == LayerMask.NameToLayer("Particles"))
			return;

		Unit target = other.gameObject.GetComponent<Unit>();
		if (target)
		{
			// Do nothing to same team units
			if (!friendlyFire && target.team.Equals(team))
				return;

			// Do nothing on self
			if (target.gameObject.GetInstanceID() == projectileLauncher.GetInstanceID())
				return;

			// Do nothing on dead unit
			if (target.IsDead())
				return;
			
			// If projectile launcher still alive, send message to give xp
			if (projectileLauncher)
				projectileLauncher.SendMessage("ProjectileParticleCallback", target);

			// Deal Damage
			if (areaOfEffect == 0)
				target.DealDamage(damage);
			else
				DealDamageToAllTargetsInArea(areaOfEffect, damage);

			// Show particles on unit
			if (oneShot)
				DestroyParticles();
			else if (showHitParticle)
				hitParticle.Emit(1);
		}
		// If hit ground, one shot does not matter anymore, just destroy
		else
		{
			if (areaOfEffect > 0)
				DealDamageToAllTargetsInArea(areaOfEffect, damage);

			DestroyParticles();
		}
	}

	private void DealDamageToAllTargetsInArea(float area, float damage)
	{
		Collider[] colls = Physics.OverlapSphere(transform.position, area / 2, LayerMask.GetMask("Selectable"));
		foreach (var col in colls)
		{
			Unit unit = col.gameObject.GetComponent<Unit>();

			// Do nothing for dead units, and same team units with friendly fire off
			if (!unit || unit.IsDead())
				continue;
			if (unit.team.Equals(team) && !friendlyFire)
				continue;

			unit.DealDamage(damage);
		}
	}
}
