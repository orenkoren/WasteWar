using UnityEngine;

public class RuntimeGameObjRefs : MonoBehaviour
{
    public Terrain TERRAIN;
    public static RuntimeGameObjRefs Instance { get; private set; }

    private void Awake()
    {
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
