using UnityEngine;
using System.Collections;

public class ShowHealthBars : MonoBehaviour
{

    private SelectRectangle selectRectangle;

    // Use this for initialization
    void Start()
    {
        selectRectangle = GameObject.FindGameObjectWithTag("GameController").GetComponent<SelectRectangle>();
    }

    // Update is called once per frame
    void Update()
    {
        var allUnits = Data.GetInstance().GetAllUnits();
        foreach (var unit in allUnits)
        {
            unit.GetComponent<HealthBar>().showBar = false;
        }

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
#else
		if (Input.GetButton("Show Stats"))
#endif
        {
            foreach (var unit in allUnits)
            {
                unit.GetComponent<HealthBar>().showBar = true;
            }
        }
        else
        {
            if (selectRectangle.GetSelectedUnits().Count > 0)
            {
                var allSelectedUnits = selectRectangle.GetSelectedUnits();
                foreach (var unit in allSelectedUnits)
                {
                    unit.GetComponent<HealthBar>().showBar = true;
                }
            }
            var go = Common.GetObjectUnderMouse(true);
            var unitUnderMouse = (go) ? go.GetComponent<Unit>() : null;
            if (unitUnderMouse) unitUnderMouse.GetComponent<HealthBar>().showBar = true;
        }
    }
}
