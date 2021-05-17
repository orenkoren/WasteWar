using System.Collections;
using UnityEngine;

public class SpawnLaserBeams : MonoBehaviour
{
    public Transform spawnLocation;
    public GameObject beamPrefab;
    public AudioSource soundSource;
    public float lifetimeInSeconds;

    private void Start()
    {
        //StartCoroutine(ShootBeam());
    }

    IEnumerator ShootBeam()
    {
        while (true)
        {
            SpawnBeam();
            yield return new WaitForSeconds(1);
        }
    }

    public void SpawnBeam()
    {
        GameObject beam = Instantiate(beamPrefab, spawnLocation);
        if (soundSource)
            soundSource.Play();
        StartCoroutine(DestroyBeam(beam));
    }

    IEnumerator DestroyBeam(GameObject beam)
    {
        yield return new WaitForSeconds(lifetimeInSeconds);
        Destroy(beam);
    }
}
