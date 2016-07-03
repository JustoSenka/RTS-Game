using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class FpsWriter : MonoBehaviour
{
    private Text text;
    private long frameCount = 0;
    private float timePassed = 0;
    private float fps = 0;
    private float updateRate = 3;

    void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        frameCount++;
        timePassed += Time.deltaTime;
        if (timePassed > 1.0 / updateRate)
        {
            fps = frameCount / timePassed;
            frameCount = 0;
            timePassed = 0;
        }

        if (text != null)
        {
            text.text = "FPS: " + (int)fps;
        }
    }
}
