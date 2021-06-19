using UnityEngine;

public class ActiveCamera : MonoBehaviour
{
    public Camera defaultCamera;
    public Camera alternativeCamera;
    public Camera activeCam;

    void Awake()
    {
        defaultCamera.GetComponent<AudioListener>().enabled = true;
        alternativeCamera.enabled = false;
        activeCam = defaultCamera;
    }

    void Start()
    {
        GameEvents.MiddleMouseClickPressedListeners += SetCamera;
    }

    public void SetCamera(object sender, int data)
    {
        activeCam.GetComponent<AudioListener>().enabled = false;
        activeCam.enabled = false;
        if (activeCam.name == defaultCamera.name)
        {
            activeCam = alternativeCamera;
        }
        else
        {
            activeCam = defaultCamera;
        }
        activeCam.enabled = true;
        activeCam.GetComponent<AudioListener>().enabled = true;
    }

    private void OnDestroy()
    {
        GameEvents.MiddleMouseClickPressedListeners -= SetCamera;
    }
}
