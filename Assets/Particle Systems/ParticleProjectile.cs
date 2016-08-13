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

	public void Launch(Vector3 pos, float speed)
	{
		this.RunAfterOneFrame(() => rb.velocity = Vector3.Scale((pos - transform.position), new Vector3(1, 0, 1)).normalized * speed);
	}

	public void DestroyParticles()
	{
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
		ps.GetParticles(particles);

		for (int i = 0; i < particles.Length; i++)
		{
			particles[i].lifetime = 0;
		}
	}

	public void OnCollisionEnter(Collision collision)
	{
		Unit target = collision.collider.gameObject.GetComponent<Unit>();
		if (target)
		{
			// Do nothing to same team units
			if (!friendlyFire && target.team.Equals(team))
				return;

			projectileLauncher.SendMessage("ProjectileParticleCallback", target);
		}

		if (showHitParticle)
		{
			hitParticle.Emit(1);
			if (oneShot)
				Particles.Instance.RunAfter(hitParticle.duration, () => Destroy(this));
		}
		else if (oneShot)
		{
			DestroyParticles();
			Destroy(this);
		}

		//@TODO Destroy after some duration went out?
	}
}
