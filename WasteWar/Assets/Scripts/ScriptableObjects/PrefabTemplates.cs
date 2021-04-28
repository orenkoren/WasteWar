using UnityEngine;

[CreateAssetMenu(fileName = "PrefabTemplates", menuName = "scriptableObjects",order =2)]
public class PrefabTemplates : ScriptableObject
{
    public GameObject Building;
    public GameObject Turret;
    public GameObject Wall;
    public GameObject PowerPole;
    public GameObject PipeTopBottom;
    public GameObject PipeLeftRight;
    public GameObject PipeTopRight;
    public GameObject PipeBottomRight;
    public GameObject PipeBottomLeft;
    public GameObject PipeTopLeft;
}
