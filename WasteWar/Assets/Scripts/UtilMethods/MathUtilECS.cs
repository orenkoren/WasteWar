using System;
using Unity.Mathematics;
using UnityEngine;

public static class MathUtilECS
{
    public static float3 MoveTowards(float3 current, float3 target, float maxDistanceDelta)
    {
        float deltaX = target.x - current.x;
        float deltaY = target.y - current.y;
        float deltaZ = target.z - current.z;
        float sqdist = deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ;
        if (sqdist == 0 || sqdist <= maxDistanceDelta * maxDistanceDelta)
            return target;
        var dist = (float)Math.Sqrt(sqdist);
        return new float3(current.x + deltaX / dist * maxDistanceDelta,
            current.y + deltaY / dist * maxDistanceDelta,
            current.z + deltaZ / dist * maxDistanceDelta);
    }

    public static float3 MoveTowardsV2(float3 current, float3 target, float maxDistanceDelta)
    {
        float num1 = target.x - current.x;
        float num2 = target.y - current.y;
        float num3 = target.z - current.z;
        float num4 = (float)(num1 * (double)num1 + num2 * (double)num2 + num3 * (double)num3);
        if (num4 == 0.0 || num4 <= maxDistanceDelta * (double)maxDistanceDelta)
            return target;
        float num5 = (float)Math.Sqrt(num4);
        return new float3(current.x + num1 / num5 * maxDistanceDelta, current.y + num2 / num5 * maxDistanceDelta, current.z + num3 / num5 * maxDistanceDelta);
    }

    public static Vector3 FromXZPlane(Vector2 vec)
    {
        return new Vector3 { x = vec.x, y = 0, z = vec.y };
    }

    public static float3 FromXZPlane(float2 vec)
    {
        return new float3 { x = vec.x, y = 0, z = vec.y };
    }

    public static float2 ToXZPlane(float3 vec)
    {
        return new float2 { x = vec.x, y = vec.z };
    }
}
