using UnityEngine;

public static class GridUtils
{
    public static GridUtils.GridCoords Vector3ToGridCoord(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int z = Mathf.FloorToInt(pos.z);

        GridUtils.GridCoords gridPos = new GridUtils.GridCoords(x, z);

        return gridPos;
    }

    public struct GridCoords
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public GridCoords(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
