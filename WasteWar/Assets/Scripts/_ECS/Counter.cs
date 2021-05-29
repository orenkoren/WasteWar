using UnityEngine;

public class Counter : MonoBehaviour
{
    public float SecondsInterval;
    [HideInInspector]
    public float currentTime;

    private void Awake()
    {
        currentTime = SecondsInterval;
    }

    void Update()
    {
        currentTime -= Time.deltaTime;
    }

    public void ResetTimer()
    {
        currentTime = SecondsInterval;
    }
}
