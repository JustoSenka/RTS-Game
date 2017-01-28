using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleSystemPlayer : MonoBehaviour
{
	private ParticleSystem ps;
	private ParticleSystem[] pss;

	private float stopAfter;
	private bool shouldStop = false;

	void Update()
	{
		if (shouldStop)
		{
			stopAfter -= Time.deltaTime;
			if (stopAfter <= 0)
			{
				shouldStop = false;
				Stop();
			}
		}
	}

    void Start()
    {
		ps = GetComponent<ParticleSystem>();
		pss = GetComponentsInChildren<ParticleSystem>();

        if (pss[0].playOnAwake)
        {
            Play();
        }
    }

	public void SetPosition(Vector3 pos)
	{
		transform.position = pos;
	}

    public void Play()
    {
        foreach (var p in pss) p.Play();
    }

	public void Play(float time)
	{
		stopAfter = time;
		shouldStop = true;
		foreach (var p in pss) p.Play();
	}

	public void Stop()
    {
        foreach (var p in pss) p.Stop();
    }

	public void DestroyParticles()
	{
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
		ps.GetParticles(particles);

		for (int i = 0; i < particles.Length; i++)
		{
			particles[i].remainingLifetime = 0;
		}
	}
}
