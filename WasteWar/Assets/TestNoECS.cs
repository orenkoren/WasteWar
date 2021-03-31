using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNoECS : MonoBehaviour
{
    public GameObject prefab;
    public float amount;

    private List<GameObject> existing = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < amount; i++)
        {
            existing.Add(Instantiate(prefab));
        }
    }

    private void Update()
    {
        //foreach (var item in existing)
        //{
        //    item.transform.position = new Vector3(Random.Range(0, 100), 0, Random.Range(0, 100));
        //}
    }
}
