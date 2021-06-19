using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSync : MonoBehaviour
{
    public event Action<float> OnHealthChanged = delegate { };

    public void SyncHealth(float newHealthPercentage)
    {
        OnHealthChanged(newHealthPercentage);
    }
}
