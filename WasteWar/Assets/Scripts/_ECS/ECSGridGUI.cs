using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class ECSGridGUI : MonoBehaviour
{
    public bool displayGrid;
    public bool displayBestCostField;
    public bool displayIndicies;
    public bool displayBaseCosts;
    [HideInInspector]
    public GridCell[,] grid = new GridCell[0, 0];

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        for (int x = 0; x < grid.GetLength(0) - 1; x++)
        {
            for (int z = 0; z < grid.GetLength(1) - 1; z++)
            {
                Vector3 centerPos = MathUtilECS.FromXZPlane(grid[x, z].centerPos);
                Vector3 bottomLeftPos = MathUtilECS.FromXZPlane(grid[x, z].bottomLeftPos);
                if (displayGrid)
                {
                    //if (grid[x, z].cost == 255)
                    //{
                        Debug.DrawLine(bottomLeftPos, MathUtilECS.FromXZPlane(grid[x + 1, z].bottomLeftPos));
                        Debug.DrawLine(bottomLeftPos, MathUtilECS.FromXZPlane(grid[x, z + 1].bottomLeftPos));
                        Debug.DrawLine(MathUtilECS.FromXZPlane(grid[x + 1, z].bottomLeftPos), MathUtilECS.FromXZPlane(grid[x + 1, z + 1].bottomLeftPos));
                        Debug.DrawLine(MathUtilECS.FromXZPlane(grid[x, z + 1].bottomLeftPos), MathUtilECS.FromXZPlane(grid[x + 1, z + 1].bottomLeftPos));
                    //}
                }

                if (displayBestCostField)
                    Handles.Label(centerPos, grid[x, z].bestCost.ToString());
                else if(displayBaseCosts)
                    Handles.Label(centerPos, grid[x, z].cost.ToString());
                if (displayIndicies)
                    Handles.Label(bottomLeftPos, x.ToString() + "," + z.ToString());


            }
        }
    }
#endif
}

