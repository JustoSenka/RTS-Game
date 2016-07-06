using UnityEngine;
using System.Collections;

public class MonoBehaviourSlowUpdates : MonoBehaviour {

    protected float slowUpdateFixedDeltaTime = 0.1f;
    protected float slowerUpdateFixedDeltaTime = 0.2f;
    protected float slowestUpdateFixedDeltaTime = 0.4f;

    private float m_slowUpdateTimeLeft = 1;
    private float m_slowerUpdateTimeLeft = 1;
    private float m_slowestUpdateTimeLeft = 1;
	
	private void Update ()
    {
        BeforeUpdate();

        var deltaTime = Time.deltaTime;

        m_slowUpdateTimeLeft -= deltaTime;
        m_slowerUpdateTimeLeft -= deltaTime;
        m_slowestUpdateTimeLeft -= deltaTime;

        if (m_slowUpdateTimeLeft <= 0)
        {
            SlowUpdate();
            m_slowUpdateTimeLeft += slowUpdateFixedDeltaTime;
        }

        if (m_slowerUpdateTimeLeft <= 0)
        {
            SlowerUpdate();
            m_slowerUpdateTimeLeft += slowerUpdateFixedDeltaTime;
        }

        if (m_slowestUpdateTimeLeft <= 0)
        {
            SlowestUpdate();
            m_slowestUpdateTimeLeft += slowestUpdateFixedDeltaTime;
        }

        AfterUpdate();
    }

    protected virtual void BeforeUpdate()
    {
        
    }

    protected virtual void AfterUpdate()
    {

    }

    protected virtual void SlowUpdate()
    {

    }

    protected virtual void SlowerUpdate()
    {

    }

    protected virtual void SlowestUpdate()
    {

    }

    public void DelaySlowUpdateBy(float time)
    {
        m_slowUpdateTimeLeft += time;
    }

    public void DelaySlowerUpdateBy(float time)
    {
        m_slowerUpdateTimeLeft += time;
    }

    public void DelaySlowestUpdateBy(float time)
    {
        m_slowestUpdateTimeLeft += time;
    }
}
