using UnityEngine;

public class CollisionWithLayer : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("kekw");
        if (collision.gameObject.layer == LayerMasks.Instance.ATTACKABLE)
            Debug.Log("kekw");
    }
}
