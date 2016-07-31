using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Unit))]
public class HealthBar : MonoBehaviour
{
    [System.NonSerialized]
	public bool showBar = true;

    public Canvas canvas;
    public RectTransform healthImage;
    public RectTransform manaImage;

    private Unit unit;

    private bool lastShowBar = false;
    private float lastHp;
    private float lastMp;

	void Start()
	{
		unit = GetComponent<Unit>();
        canvas.transform.localPosition = new Vector3(0, unit.height, 0);
        canvas.enabled = lastShowBar;
	}

    void Update()
    {
        if (healthImage && unit.hp != lastHp)
        {
            var scale = healthImage.localScale;
            scale.x = unit.hp / unit.maxHp;
			if (scale.x < 0) scale.x = 0;
            healthImage.localScale = scale;
            lastHp = unit.hp;
        }
        if (manaImage && unit.mp != lastMp)
        {
            var scale = manaImage.localScale;
            scale.x = unit.mp / unit.maxMp;
			if (scale.x < 0) scale.x = 0;
			manaImage.localScale = scale;
            lastMp = unit.mp;
        }
        if (lastShowBar != showBar)
        {
            canvas.enabled = showBar;
            lastShowBar = showBar;
        }
    }
}
