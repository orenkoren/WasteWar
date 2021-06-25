using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class RotateHybridSync : MonoBehaviour
{
    public Transform customTargetSync;
    public LerpBehavior rotationBehavior;

    private bool isReady = true;
    private delegate float LerpFunction(float t);

    public void RotateHybrid(float3 targetLocation, float overTime)
    {
        if (!isReady || Equals(targetLocation, float3.zero))
            return;
        var rot1 = customTargetSync.rotation;
        Vector3 directionToLookAt = customTargetSync.position -
                                    new Vector3(targetLocation.x, targetLocation.y, targetLocation.z);
        var rot2 = Quaternion.LookRotation(-directionToLookAt);
        var a = Quaternion.Angle(rot1, rot2);
        if (a < 1) return;
        isReady = false;
        StartCoroutine(RotateOverTime(rot1, rot2, overTime));
    }

    IEnumerator RotateOverTime(Quaternion start, Quaternion end, float dur)
    {
        float t = 0f;
        LerpFunction lerpFunc = (LerpFunction)Delegate.CreateDelegate(typeof(LerpFunction), this,
                                                GetType().GetMethod(rotationBehavior.ToString()));
        while (t < dur)
        {
            customTargetSync.rotation = Quaternion.Slerp(start,
                                                    end,
                                                    lerpFunc(t / dur));
            yield return null;
            t += Time.deltaTime;
        }
        customTargetSync.rotation = end;
        isReady = true;
    }

    public float EaseIn(float t)
    {
        return t * t;
    }

    public float Flip(float x)
    {
        return 1 - x;
    }

    public float EaseOut(float t)
    {
        return Flip(Mathf.Sqrt(Flip(t)));
    }

    public float EaseInOut(float t)
    {
        return Mathf.Lerp(EaseIn(t), EaseOut(t), t);
    }

    public float Spike(float t)
    {
        if (t <= .5f)
            return EaseIn(t / .5f);

        return EaseIn(Flip(t) / .5f);
    }
}

public enum LerpBehavior
{
    EaseIn,
    EaseOut,
    EaseInOut,
    Flip,
    Spike
}