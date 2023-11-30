using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour {

    Transform trans;
    Transform mainCameraTrans;

	void Start () {
        trans = transform;
        mainCameraTrans = Camera.main.transform;
	}
	
	void Update () {
        Quaternion rot = mainCameraTrans.rotation;
        trans.rotation = Quaternion.Euler(rot.eulerAngles.x, 0, 0);
	}
}
