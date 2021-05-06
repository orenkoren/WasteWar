using UnityEngine;

public class LayerMasks : MonoBehaviour
{

    //layermasks
    public int IGNORE_RAYCAST { get; private set; }
    public int GROUND { get; private set; }
    public int RESOURCE { get; private set; }
    public int ATTACKABLE { get; private set; }
    //layers
    public int IGNORE_RAYCAST_LAYER { get; private set; }
    public int GROUND_LAYER { get; private set; }
    public int RESOURCE_LAYER { get; private set; }
    public int ATTACKABLE_LAYER { get; private set; }

    public static LayerMasks Instance { get; private set; }

    private void Awake()
    {
        IGNORE_RAYCAST = LayerMask.GetMask("Ignore Raycast");
        GROUND = LayerMask.GetMask("Ground");
        RESOURCE = LayerMask.GetMask("Resource");
        ATTACKABLE = LayerMask.GetMask("Attackable");

        IGNORE_RAYCAST_LAYER = LayerMask.NameToLayer("Ignore Raycast");
        GROUND_LAYER = LayerMask.NameToLayer("Ground");
        RESOURCE_LAYER = LayerMask.NameToLayer("Resource");
        ATTACKABLE_LAYER = LayerMask.NameToLayer("Attackable");

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
    }
}
