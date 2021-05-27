using System.Collections;
using UnityEngine;

public class SpawnLaserBeams : MonoBehaviour
{
    public AudioSource soundSource;


    public void PlayBeamSound()
    {
        if (soundSource)
            soundSource.Play();
    }
}
