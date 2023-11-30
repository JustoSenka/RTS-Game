using UnityEngine;
using UnityEngine.Serialization;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Data : MonoBehaviour
{
    public static Data Instance { get; private set; }

    public Unit[] Units { get; private set; } = new Unit[0];

    [FormerlySerializedAs("UnitColors")]
    public Color[] unitColors;
    public Texture[] skillIconTextures;
    public Skill[] skills;

    private long frame = 0;

    void Awake()
    {
        Instance = this;
    }

    public void Update()
    {
        frame++;
        if (frame % 5 != 0)
            return;

        Units = GameObject.FindObjectsOfType<Unit>(includeInactive: false);
    }
}
