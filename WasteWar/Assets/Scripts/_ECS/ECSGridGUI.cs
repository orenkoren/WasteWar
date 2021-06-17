using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class ECSGridGUI : MonoBehaviour
{
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
