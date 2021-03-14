using UnityEngine;
using System.Collections.Generic;

//https://gist.github.com/mdomrach/a66602ee85ce45f8860c36b2ad31ea14


public class TerrainGridMesh : MonoBehaviour
{
    public Terrain terrain;
    //can make it visible in the inspector if necessary but it shouldn't be because the grid covers the whole terrain
    private int GridSize;
    private float cellLoc = 0f;

    void Awake()
    {
        GridSize= (int)(terrain.GetComponent<Terrain>().terrainData.size.x);

        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        var mesh = new Mesh();
        var verticies = new List<Vector3>();

        var indicies = new List<int>();

        //logic @ https://i.imgur.com/4n2iKqP.gifv
        for (int i = 0; i <= (int)(GridSize/ GridConstants.Instance.FloatCellSize()); i++)
        {
            

            verticies.Add(new Vector3(cellLoc, 0, 0));
            verticies.Add(new Vector3(cellLoc, 0, GridSize));

            indicies.Add(4 * i + 0);
            indicies.Add(4 * i + 1);

            verticies.Add(new Vector3(0, 0, cellLoc));
            verticies.Add(new Vector3(GridSize, 0, cellLoc));

            indicies.Add(4 * i + 2);
            indicies.Add(4 * i + 3);

            cellLoc += GridConstants.Instance.FloatCellSize();
        }

        mesh.vertices = verticies.ToArray();
        mesh.SetIndices(indicies.ToArray(), MeshTopology.Lines, 0);
        filter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.material.color = Color.white;
    }
}