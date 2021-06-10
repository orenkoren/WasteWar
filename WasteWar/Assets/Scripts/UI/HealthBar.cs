using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image foregroundImage;
    public HealthSync syncComp;
    public Camera cam;

    private void Awake()
    {
        if (!cam)
            cam = Camera.main;
    }
    void Start()
    {
        syncComp.OnHealthChanged += ChangeFillAmount;
    }

    private void ChangeFillAmount(float percentage)
    {
        foregroundImage.fillAmount = percentage / 100;
    }

    private void LateUpdate()
    {
        transform.LookAt(cam.transform);
    }
}
