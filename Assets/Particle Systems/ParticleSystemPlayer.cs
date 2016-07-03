using UnityEngine;
using System.Collections;

public class ParticleSystemPlayer : MonoBehaviour
{
    private ParticleSystem[] ps;

    void Start()
    {
        ps = GetComponentsInChildren<ParticleSystem>();

        if (ps[0].playOnAwake)
        {
            Play();
        }
    }

    public void Play()
    {
        foreach (var p in ps) p.Play();
    }

    public void Stop()
    {
        foreach (var p in ps) p.Stop();
    }
}
