using UnityEngine;

public class HybridEntitySync : MonoBehaviour
{
    public void DestroyHybrid()
    {
        print("destroying gameobject" + gameObject.name);
        Destroy(gameObject);
    }
}
