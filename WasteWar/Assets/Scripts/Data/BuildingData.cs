using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class BuildingData : MonoBehaviour
{
    public ResourceGrid Resources { get; set; } = null;
    public int PollutionIndex { get; private set; } = 1;
    public int Health { get; private set; } = 100;
    public int Armor { get; private set; } = 1;
    public int Level { get; private set; } = 1;
    public bool IsTemplate { get; set; } = true;

    private GridUtils.GridCoords buildingLoc;
    private int _storage = 0;
    private float _yieldFrequency = 1.1f;
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
    {                                                                       // just Text, doesn't work
        CubeTextComponent = gameObject.transform.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Text>();
        buildingLoc = GridUtils.Vector3ToGridCoord(gameObject.GetComponent<Collider>().bounds.min);
        StartCoroutine(MineResource());
    }

    private IEnumerator MineResource()
    {
        while ( !CheckIfStorageFull() && !IsTemplate && CheckForResourcesBelow(buildingLoc))
        {
           CubeTextComponent.text=(++_storage).ToString();
           //TODO place this somewhere else
           Resources.Nodes[key].Count--;

            yield return new WaitForSeconds(YieldFrequency);
        }
    }

    private bool CheckForResourcesBelow(GridUtils.GridCoords buildingLoc)
    {
        key = buildingLoc.X * MathUtils.DICT_KEY_GENERATOR + buildingLoc.Y;

        if (Resources.Nodes.ContainsKey(key))
            return true;
        else
            return false;
    }

    private bool CheckIfStorageFull()
    {
        return _storage == 10;
    }
}
