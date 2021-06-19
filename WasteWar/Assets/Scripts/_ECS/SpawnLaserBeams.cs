using System.Collections;
using UnityEngine;

public class SpawnLaserBeams : MonoBehaviour
{
    public AudioSource soundSource;
    public AudioClip[] laserSounds;


    public void PlayBeamSound()
    {
        if (soundSource)
        {
            soundSource.pitch = Random.Range(0.8f, 1.2f);
            soundSource.clip = laserSounds[Random.Range(0, laserSounds.Length - 1)];
            soundSource.Play();
        }
    }
}
