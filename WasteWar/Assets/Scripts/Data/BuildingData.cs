using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingData : MonoBehaviour
{
    private const int TOTAL_CAPACITY = 100;
    private const float STARTING_YIELD_SPEED = 1.1f;

    public ResourceGrid Resources { get; set; } = null;
    public int PollutionIndex { get; private set; } = 1;
    public int Health { get; private set; } = 100;
    public int Armor { get; private set; } = 1;
    public int Level { get; private set; } = 1;
    public bool IsTemplate { get; set; } = true;

    private int _storage = 0;
    private float _yieldFrequency = STARTING_YIELD_SPEED;
    private Stack<int> AvailableResources = new Stack<int>();

    private GridUtils.GridCoords buildingLoc;
    private GridUtils.GridCoords buildingSize;
    private int key;
    private UnityEngine.UI.Text CubeTextComponent;

    public float YieldFrequency
    {
        get
        {
            return _yieldFrequency;
        }
        private set
        {
            _yieldFrequency -= _yieldFrequency * (value / 10);
        }
    }

    private void Start()
    {
        if (!IsTemplate)
        {
            CubeTextComponent = gameObject.transform.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Text>();
            buildingLoc = GridUtils.Vector3ToGridCoord(gameObject.GetComponent<Collider>().bounds.min);
            buildingSize = GridUtils.Vector3ToGridCoord(gameObject.GetComponent<Collider>().bounds.size);

            GetResourcesBelowBuilding(buildingLoc, buildingSize);
            StartCoroutine(MineResource());
        }
    }

    private IEnumerator MineResource()
    {
        while (!CheckIfStorageFull() && AvailableResources.Count != 0)
        {
            CubeTextComponent.text = (++_storage).ToString();
            key = AvailableResources.Peek();

            Resources.Nodes[key].Count--;
            if (Resources.Nodes[key].Count == 0)
            {
                Resources.Nodes.Remove(key);
                GameEvents.FireNodeUsedUp(this, key);
                AvailableResources.Pop();
            }
            yield return new WaitForSeconds(YieldFrequency);
        }
    }

    private void GetResourcesBelowBuilding(GridUtils.GridCoords buildingLoc, GridUtils.GridCoords buildingSize)
    {
        for (int i = 0; i <= buildingSize.X; i++)
        {
            for (int j = 0; j <= buildingSize.Y; j++)
            {
                key = (buildingLoc.X + i) * MathUtils.DICT_KEY_GENERATOR + buildingLoc.Y + j;
                if (Resources.Nodes.ContainsKey(key) && Resources.Nodes[key].Count != 0)
                    AvailableResources.Push(key);
            }
        }
    }

    private bool CheckIfStorageFull()
    {
        return _storage == TOTAL_CAPACITY;
    }
}
