using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTargetFramerate : MonoBehaviour
{
    public int targetFps = 60;

    void Start()
    {
        Application.targetFrameRate = targetFps;
    }
}
