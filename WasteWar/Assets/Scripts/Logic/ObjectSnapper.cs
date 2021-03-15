using UnityEngine;

public static class ObjectSnapper
{
   
    //incase cell size is 1
    public static Vector3 SnapToGridCell(Vector3 currPos)
    {
        float x = Mathf.Floor(currPos.x);
        float z = Mathf.Floor(currPos.z);

        return new Vector3(x, currPos.y, z);
    }
    // https://gyazo.com/c82e819f59704c25345a5474f555ecca
    public static Vector3 SnapToGridCell(Vector3 currPos,float CellSize)
    {
            float x = Mathf.Floor(currPos.x / CellSize) * CellSize;
            float z = Mathf.Floor(currPos.z / CellSize) * CellSize;

            return new Vector3(x,currPos.y,z);
    }

    // https://gyazo.com/1cea52338703354da4a57cb6cabc7d12
    public static Vector3 SnapToGridCell(Vector3 currPos, float CellSize,Vector3 objectSize)
    {
        float x = Mathf.Floor(currPos.x / CellSize) * CellSize + objectSize.x/2;
        float y = 0 + objectSize.y / 2;
        float z = Mathf.Floor(currPos.z / CellSize) * CellSize + objectSize.z/2;

        return new Vector3(x, y, z);
    }
}
