using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingLogic : MonoBehaviour
{
    [SerializeField]
    private GameObject resources;

    private Dictionary<int, Resource> Nodes { get; set; }

    private void Start()
    {
        Nodes = resources.GetComponent<ResourceGrid>().Nodes;
        GameEvents.BuildingPlacedListeners += StartBuildingProcesses;
    }

    private void StartBuildingProcesses(object sender, GameObject building)
    {
        GetResourcesBelowBuilding(building);
        StartCoroutine(MineResource(building));
    }

    private IEnumerator MineResource(GameObject building)
    {
        BuildingState data = building.GetComponent<BuildingState>();
        while (!CheckIfStorageFull(building) && data.AvailableResources.Count > 0)
        {
            data.CubeTextComponent.text = (++data._storage).ToString();
            data.AvailableResources.Count--;
            if (data.AvailableResources.Count <= 0)
                GameEvents.FireNodeUsedUp(this, data.KeyOfNodeBeingUsed);

            yield return new WaitForSeconds(data.YieldFrequency);
        }
    }

    private void GetResourcesBelowBuilding(GameObject building)
    {
        GridUtils.GridCoords buildingLoc = GridUtils.Vector3ToGridCoord(building.GetComponent<Collider>().bounds.min);
        BuildingState data = building.GetComponent<BuildingState>();

        data.KeyOfNodeBeingUsed = (buildingLoc.X) * GridConstants.Instance.CELL_COUNT + buildingLoc.Y;
        if (Nodes.ContainsKey(data.KeyOfNodeBeingUsed) && Nodes[data.KeyOfNodeBeingUsed].Count > 0)
            data.AvailableResources = Nodes[data.KeyOfNodeBeingUsed];
    }

    private bool CheckIfStorageFull(GameObject building)
    {
        BuildingState data = building.GetComponent<BuildingState>();
        return data._storage >= data.TOTAL_CAPACITY;
    }

    private void OnDestroy()
    {
        GameEvents.BuildingPlacedListeners -= StartBuildingProcesses;
    }
}
