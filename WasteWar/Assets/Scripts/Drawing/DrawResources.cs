using System.Collections.Generic;
using UnityEngine;

public class DrawResources : MonoBehaviour
{
    [SerializeField]
    private GameObject resourceTemplate;
    private List<GameObject> resourceStructures = new List<GameObject>();


    private void Awake()
    {
        GameEvents.ResourcesGeneratedListeners += InstantiateResourceGameObjects;
        GameEvents.EraseResourceGameObjectListeners += UnInstantiateStructure;
    }

    private void InstantiateResourceGameObjects(object sender,Dictionary<int,Resource> resources)
    {
        foreach (var resource in resources)
        {
            Vector3 resourcePos = new Vector3(resource.Key / GridConstants.Instance.CELL_COUNT,
                                              0,
                                              resource.Key % GridConstants.Instance.CELL_COUNT
                                              );
            GameObject structure = Instantiate(resourceTemplate,
                                               ObjectSnapper.SnapToGridCell(resourcePos, resourceTemplate.GetComponent<MeshRenderer>().bounds.size),
                                               Quaternion.Euler(GameConstants.Instance.DEFAULT_OBJECT_ROTATION)
                                               );
            structure.GetComponent<ResourceData>().Key = resource.Key;
            resourceStructures.Add(structure);
        }
    }
    private void UnInstantiateStructure(object sender, int key)
    {
        GameObject structure = resourceStructures.Find(el => el.GetComponent<ResourceData>().Key == key);
        resourceStructures.Remove(structure);
        Destroy(structure);
    }

    private void OnDestroy()
    {
        GameEvents.ResourcesGeneratedListeners -= InstantiateResourceGameObjects;
        GameEvents.EraseResourceGameObjectListeners -= UnInstantiateStructure;
    }
}
