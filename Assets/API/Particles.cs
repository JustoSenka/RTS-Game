using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Particles : MonoBehaviour
{
	void Awake() { Instance = this; }
	private static Particles _instance;
	public static Particles Instance
	{
		get
		{
			return _instance;
		}
		private set
		{
			_instance = value;
		}
	}

	public ParticleSystem this[int i]
	{
		get
		{
			return particleRefArray[i];
		}
		set
		{
			particleRefArray[i] = value;
		}
	}

	public ParticleSystem[] particleRefArray;

	// Coroutine holder here (starts on self, so they live even when units die)
	public void RunAfter(float sec, Action ac)
	{
		if (sec > 0)
		{
			this.StartCoroutine(RunAfterEnum(sec, ac));
		}
		else
		{
			ac.Invoke();
		}
	}

	public void RunAfterOneFrame(Action ac)
	{
		this.StartCoroutine(RunAfterEnum(-1, ac));
	}

	private IEnumerator RunAfterEnum(float sec, Action ac)
	{
		if (sec == -1)
		{
			yield return null;
		}
		else
		{
			yield return new WaitForSeconds(sec);
		}
		ac.Invoke();
	}
}
