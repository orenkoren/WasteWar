using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class ECSGridGUI : MonoBehaviour
{
    public bool displayGrid;
    public bool displayBestCostField;
    public bool displayIndicies;
    [HideInInspector]
    public GridCell[,] grid = new GridCell[0, 0];

    private void OnDrawGizmos()
    {
        for (int x = 0; x < grid.GetLength(0) - 1; x++)
        {
            for (int z = 0; z < grid.GetLength(1) - 1; z++)
            {
                Vector3 centerPos = XZPlane(grid[x, z].centerPos);
                Vector3 bottomLeftPos = XZPlane(grid[x, z].bottomLeftPos);
                if (displayGrid)
                {
                    //if (grid[x, z].cost == 255)
                    //{
                        Debug.DrawLine(bottomLeftPos, XZPlane(grid[x + 1, z].bottomLeftPos));
                        Debug.DrawLine(bottomLeftPos, XZPlane(grid[x, z + 1].bottomLeftPos));
                        Debug.DrawLine(XZPlane(grid[x + 1, z].bottomLeftPos), XZPlane(grid[x + 1, z + 1].bottomLeftPos));
                        Debug.DrawLine(XZPlane(grid[x, z + 1].bottomLeftPos), XZPlane(grid[x + 1, z + 1].bottomLeftPos));
                    //}
                }

                if (displayBestCostField)
                    Handles.Label(centerPos, grid[x, z].bestCost.ToString());
                else
                    Handles.Label(centerPos, grid[x, z].cost.ToString());
                if (displayIndicies)
                    Handles.Label(bottomLeftPos, x.ToString() + "," + z.ToString());


            }
        }
    }

    public static Vector3 XZPlane(Vector2 vec)
    {
        return new Vector3 { x = vec.x, y = 0, z = vec.y };
    }
}
