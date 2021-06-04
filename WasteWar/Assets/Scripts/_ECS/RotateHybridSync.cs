using System.Collections;
using UnityEngine;

public class RotateHybridSync : MonoBehaviour
{
    public Transform customTargetSync;

    private bool isReady = true;

    public void RotateHybrid(float yAngle, float overTime)
    {
        if (!isReady) return;
        var rot1 = Quaternion.Euler(0, customTargetSync.rotation.eulerAngles.y, 0);
        var rot2 = Quaternion.Euler(0, yAngle, 0);
        var a = Quaternion.Angle(rot1, rot2);
        if (a < 1) return;
        isReady = false;
        StartCoroutine(RotateOverTime(rot1, rot2, overTime));
    }

    IEnumerator RotateOverTime(Quaternion start, Quaternion end, float dur)
    {
        float t = 0f;
        while (t < dur)
        {
            customTargetSync.rotation = Quaternion.Slerp(start,
                                                    end,
                                                    EaseInOut(t / dur));
            yield return null;
            t += Time.deltaTime;
        }
        customTargetSync.rotation = end;
        isReady = true;
    }

    float EaseIn(float t)
    {
        return t * t;
    }

    float Flip(float x)
    {
        return 1 - x;
    }

    float EaseOut(float t)
    {
        return Flip(Mathf.Sqrt(Flip(t)));
    }

    float EaseInOut(float t)
    {
        return Mathf.Lerp(EaseIn(t), EaseOut(t), t);
    }

    float Spike(float t)
    {
        if (t <= .5f)
            return EaseIn(t / .5f);

        return EaseIn(Flip(t) / .5f);
    }
}
