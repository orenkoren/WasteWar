using System.Collections;
using UnityEngine;

public class StructureShake : MonoBehaviour
{
    public float duration;
    public float speed;
    public float intensity;

    public IEnumerator ShakeTemplateForXSec()
    {
        float tempDuration = duration;
        Vector3 tempPos = new Vector3(transform.position.x,transform.position.y,transform.position.z);
        float time = 0;

        while (duration >= 0)
        {
            ShakeTemplate();
            duration -= Time.deltaTime;
            yield return null;
        }
        transform.position = tempPos;
        duration = tempDuration;

        void ShakeTemplate()
        {
            transform.position = new Vector3(transform.position.x + Mathf.Sin(time * speed) * intensity,
                                                               transform.position.y,
                                                               transform.position.z + Mathf.Sin(time * speed) * intensity
                                                               );
            time += Time.deltaTime;
        }
    }
}
