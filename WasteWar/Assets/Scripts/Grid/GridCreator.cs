using UnityEngine;

public class GridCreator : MonoBehaviour
{
    void Start()
    {
        GameLogicGrid gameGrid = new GameLogicGrid();
        Debug.Log(gameGrid.elements.Length);
    }

    void Update()
    {
        //Debug.DrawLine(ray.origin, rayHit.point);
        //if (Input.GetMouseButtonDown(0))
        //{
        //    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    Physics.Raycast(ray, out rayHit, Mathf.Infinity, ground);
        //    print(rayHit.point);
        //    grid.SetValue(rayHit.point, 10);
        //}

        //if (Input.GetMouseButtonDown(1))
        //{
        //    Debug.Log(grid.GetValue(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        //}
    }
}
