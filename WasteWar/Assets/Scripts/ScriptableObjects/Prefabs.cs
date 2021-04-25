using UnityEngine;

[CreateAssetMenu(fileName = "Pipes", menuName = "scriptableObjects")]
public class Prefabs : ScriptableObject
{
    public GameObject TopBottom;
    public GameObject LeftRight;
    public GameObject TopRight;
    public GameObject BottomRight;
    public GameObject BottomLeft;
    public GameObject TopLeft;
}
