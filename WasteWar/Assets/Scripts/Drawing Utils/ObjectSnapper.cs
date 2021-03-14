using UnityEngine;

public static class ObjectSnapper
{
    public static Vector3 SnapToGridCell(Vector3 currPos,float CellSize)
    {
            float x = Mathf.Floor(currPos.x / CellSize);
            float z = Mathf.Floor(currPos.z / CellSize);

            return new Vector3(x,currPos.y,z);
    }
    public static Vector3 SnapToGridCell(Vector3 currPos, float CellSize,Vector3 objectSize)
    {
        float x = Mathf.Floor(currPos.x / CellSize) + objectSize.x/2;
        float z = Mathf.Floor(currPos.z / CellSize) + objectSize.z/2;

        return new Vector3(x, currPos.y, z);
    }
}
