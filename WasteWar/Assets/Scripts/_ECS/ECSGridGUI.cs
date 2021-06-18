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
                if (displayGrid)
                {
                    if (grid[x, z].cost == 255)
                    {
                        Debug.DrawLine(grid[x, z].bottomLeftPos, grid[x + 1, z].bottomLeftPos);
                        Debug.DrawLine(grid[x, z].bottomLeftPos, grid[x, z + 1].bottomLeftPos);
                        Debug.DrawLine(grid[x + 1, z].bottomLeftPos, grid[x + 1, z + 1].bottomLeftPos);
                        Debug.DrawLine(grid[x, z + 1].bottomLeftPos, grid[x + 1, z + 1].bottomLeftPos);
                    }
                }

                if (displayBestCostField)
                    Handles.Label(grid[x, z].centerPos, grid[x, z].bestCost.ToString());
                else
                    Handles.Label(grid[x, z].centerPos, grid[x, z].cost.ToString());
                if (displayIndicies)
                    Handles.Label(grid[x, z].bottomLeftPos, x.ToString() + "," + z.ToString());


            }
        }
    }
}
