using System.Collections;
using UnityEngine;

public class HybridDestructionSync : MonoBehaviour
{
    public void DestroyHybrid()
    {
        print("destroying gameobject" + gameObject.name);
        Destroy(gameObject);
    }
}
