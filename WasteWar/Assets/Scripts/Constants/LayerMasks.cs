using UnityEngine;

public class LayerMasks : MonoBehaviour
{
    //need to make it binary in the inspector
    public int GROUND = 1<<8;
    public static LayerMasks Instance { get; private set; }

    //????????
    private void Awake()
    {
        if (Instance == null)
        {
            //what's this here?
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
    }
}
