using UnityEngine;

public class GridCreator : MonoBehaviour
{
    public Terrain terrain;
    //not a primitive value? so why out?
    private RaycastHit hit;
    
    void Start()
    {
        
        //GameLogicGrid gameGrid = new GameLogicGrid(terrain);
        //Debug.Log(gameGrid.elements.Length);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log(hit.point);
            }
            //if (Input.GetMouseButtonDown(1))
            //{
            //    Debug.Log(grid.GetValue(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
            //}
        }
        //Debug.DrawLine(Camera.main.transform.position, hit.point, Color.blue);
    }
}