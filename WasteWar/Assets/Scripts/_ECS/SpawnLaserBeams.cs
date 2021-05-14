using System.Collections;
using UnityEngine;

public class SpawnLaserBeams : MonoBehaviour
{
    public Transform spawnLocation;
    public GameObject beamPrefab;
    public float lifetimeInSeconds;

    private void Start()
    {
        //SpawnBeam();
    }

    public void SpawnBeam()
    {
        GameObject beam = Instantiate(beamPrefab, spawnLocation);
        StartCoroutine(DestroyBeam(beam));
    }

    IEnumerator DestroyBeam(GameObject beam)
    {
        yield return new WaitForSeconds(lifetimeInSeconds);
        Destroy(beam);
    }
}
