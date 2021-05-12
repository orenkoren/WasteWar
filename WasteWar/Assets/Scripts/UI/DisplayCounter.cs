using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCounter : MonoBehaviour
{
    public Counter counter;
    public Text text;

    public int updateCooldownSeconds;

    private float currentCooldown;

    private void Awake()
    {
        currentCooldown = updateCooldownSeconds;
    }

    void Update()
    {
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0)
        {
            currentCooldown = updateCooldownSeconds;
            text.text = ((int)counter.currentTime).ToString();
        }
    }
}
