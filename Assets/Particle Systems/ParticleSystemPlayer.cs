using UnityEngine;
using System.Collections;

public class ParticleSystemPlayer : MonoBehaviour
{
    private ParticleSystem[] ps;

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
        ps = GetComponentsInChildren<ParticleSystem>();

        if (ps[0].playOnAwake)
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
        foreach (var p in ps) p.Play();
    }

	public void Play(float time)
	{
		stopAfter = time;
		shouldStop = true;
		foreach (var p in ps) p.Play();
	}

	public void Stop()
    {
        foreach (var p in ps) p.Stop();
    }
}
