using UnityEngine;
using System.Collections;

public class BuildingLogic : MonoBehaviour
{


    private void Start()
    {
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
            data.key = data.AvailableResources.Peek();

           data.Resources.Nodes[data.key].Count--;
            if (data.Resources.Nodes[data.key].Count <= 0)
            {
                data.Resources.Nodes.Remove(data.key);
                GameEvents.FireNodeUsedUp(this, data.key);
                data.AvailableResources.Pop();
            }
            yield return new WaitForSeconds(data.YieldFrequency);
        }
    }

    private void GetResourcesBelowBuilding(GameObject building)
    {
        GridUtils.GridCoords buildingLoc = GridUtils.Vector3ToGridCoord(building.GetComponent<Collider>().bounds.min);
        GridUtils.GridCoords buildingSize = GridUtils.Vector3ToGridCoord(building.GetComponent<Collider>().bounds.size);
        BuildingState data = building.GetComponent<BuildingState>();

        for (int i = 0; i <= buildingSize.X; i++)
        {
            for (int j = 0; j <= buildingSize.Y; j++)
            {
                data.key = (buildingLoc.X + i) * GridConstants.Instance.CELL_COUNT + buildingLoc.Y + j;
                if (data.Resources.Nodes.ContainsKey(data.key) && data.Resources.Nodes[data.key].Count > 0)
                    data.AvailableResources.Push(data.key);
            }
        }
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
