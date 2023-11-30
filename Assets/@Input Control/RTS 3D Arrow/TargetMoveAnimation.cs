using UnityEngine;
using System.Collections;

public class TargetMoveAnimation : MonoBehaviour
{
    public GameObject arrow;
    public float animationSpeed = 4;

    public Material greenMaterial;
    public Material redMaterial;

    private Renderer render;
    void Start()
    {
        arrow.GetComponent<Animation>()["Play"].speed = animationSpeed;
        render = GetComponentInChildren<Renderer>();
    }

    public void ShowAt(Vector3 point, bool red = false)
    {
        transform.position = point;

        if (red)
        {
            render.material = redMaterial;
        }
        else
        {
            render.material = greenMaterial;
        }

        arrow.GetComponent<Animation>().Rewind("Play");
        arrow.GetComponent<Animation>().Play("Play", PlayMode.StopAll);
    }
}
