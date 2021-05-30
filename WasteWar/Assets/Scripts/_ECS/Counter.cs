using UnityEngine;

public class Counter : MonoBehaviour
{
    public float SecondsInterval;
    public int waveCount;
    [HideInInspector]
    public float currentTime;
    [HideInInspector]
    public int currentWave = 0;

    private void Awake()
    {
        if (waveCount > 0)
            currentTime = SecondsInterval;
    }

    void Update()
    {
        if (currentTime > 0)
            currentTime -= Time.deltaTime;
        else
            currentTime = 0;
    }

    public void ResetTimer()
    {
        currentWave++;
        if (currentWave < waveCount)
            currentTime = SecondsInterval;
    }
}
