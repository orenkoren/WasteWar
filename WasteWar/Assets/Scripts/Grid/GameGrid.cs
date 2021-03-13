using UnityEngine;

public class GameGrid : MonoBehaviour
{
    public Terrain terrain;
    public float cellSize=1f;
    //not a primitive value? so why out?
    private RaycastHit hit;

    StructureGrid structureGrid;

    void Start()
    {
        structureGrid = new StructureGrid(terrain,cellSize);
    }

    void Update() { 

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                structureGrid.AddStructure(hit.point);
                Debug.Log(hit.point);
            }
        }
        Debug.DrawLine(Camera.main.transform.position, hit.point);


    }
}