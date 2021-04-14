using UnityEngine;

[CreateAssetMenu(fileName = "Pipes", menuName = "scriptableObjects")]
public class Prefabs : ScriptableObject
{
    public GameObject Pipe;
    public GameObject PipeLeftRight;
    public GameObject PipeTopRight;
    public GameObject PipeBottomRight;
    public GameObject PipeBottomLeft;
    public GameObject PipeTopLeft;
}
