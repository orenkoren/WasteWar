using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Linq;

public class ResourceGrid : MonoBehaviour
{
    [SerializeField]
    [Range(4, 50)]
    private int minimumDistance = 10;
    [SerializeField]
    private Vector2Int nodeCountMinMax= new Vector2Int(5,10);
    [SerializeField]
    [Range(0.1f,0.49f)]
    private float distanceFromMapEdgeInPercentile;
    [SerializeField]
    RuntimeGameObjRefs runtimeGameObjRefs;

    public Dictionary<int, Resource> Nodes { get; private set; } = new Dictionary<int, Resource>();

    private GridUtils.GridCoords gridSize;
    private int nodeCount;

    private void Start()
    {
        if (nodeCountMinMax.x < nodeCountMinMax.y)
            nodeCount = Random.Range(nodeCountMinMax.x, nodeCountMinMax.y);
        else
            nodeCount = Random.Range(nodeCountMinMax.y, nodeCountMinMax.x);

        gridSize = GridUtils.Vector3ToGridCoord(runtimeGameObjRefs.terrain.terrainData.size);
        GenerateResourceNodes();

        GameEvents.MouseOverListeners += ShowCurrentResourceAmount;
        GameEvents.NodeUsedUpListeners += DeleteUsedUpNode;
    }

    private void GenerateResourceNodes()
    {
        List<GridUtils.GridCoords> generatedPairs = new List<GridUtils.GridCoords>(nodeCount);

        int x;
        int y;
        GridUtils.GridCoords xyPair;
        float scaledX = gridSize.X / minimumDistance;
        float scaledY = gridSize.Y / minimumDistance;
        float scaledXOffset = scaledX * distanceFromMapEdgeInPercentile;
        float scaledYOffset = scaledY * distanceFromMapEdgeInPercentile;

        //TODO potentially refactor for perforamnce https://codereview.stackexchange.com/questions/61338/generate-random-numbers-without-repetitions

        float startTime = Time.time;

        for (int i=0;i<nodeCount;i++)
        {
            do
            {
                x = Random.Range((int)scaledXOffset, (int)(scaledX - scaledXOffset)) * minimumDistance;
                y = Random.Range((int)scaledYOffset, (int)(scaledY - scaledYOffset)) * minimumDistance;
            }
            while (generatedPairs.Any(el => el.X == x && el.Y == y));

            xyPair = new GridUtils.GridCoords(x, y);
            generatedPairs.Add(xyPair);

            Nodes.Add(x * GridConstants.Instance.CELL_COUNT + y, new Coal());
        }
        GameEvents.FireResourcesGenerated(this, Nodes);
    }

    private void DeleteUsedUpNode(object sender, int key)
    {
        Nodes.Remove(key);
        GameEvents.FireEraseResourceGameObject(this,key);
    }
    //DEBUGOUTPUT
    private void ShowCurrentResourceAmount(object sender,Vector3 hitPoint)
    {
        var gridCoord = GridUtils.Vector3ToGridCoord(hitPoint);
        int key = gridCoord.X * GridConstants.Instance.CELL_COUNT + gridCoord.Y;

        //if (Nodes.ContainsKey(key))
        //    Debug.Log(Nodes[key].Count);
    }

    private void OnDestroy()
    {
        GameEvents.MouseOverListeners -= ShowCurrentResourceAmount;
        GameEvents.NodeUsedUpListeners -= DeleteUsedUpNode;
    }
}
