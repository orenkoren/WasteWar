using UnityEngine;

[CreateAssetMenu(fileName = "PrefabPlaceable", menuName = "scriptableObjects",order =1)]
public class PrefabPlaceable : ScriptableObject
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

    public GameObject GetPlaceablePrefab(GameObject template)
    {
        if (template.CompareTag("BuildingTemplate"))
            return Building;
        else if (template.CompareTag("Turret0Template"))
            return Turret;
        else if (template.CompareTag("WallTemplate"))
            return Wall;
        else if (template.CompareTag("PowerPoleTemplate"))
            return PowerPole;
        else if (template.CompareTag("PipeTBTemplate"))
            return PipeTopBottom;
        else if (template.CompareTag("PipeLRTemplate"))
            return PipeLeftRight;
        else if (template.CompareTag("PipeTRTemplate"))
            return PipeTopRight;
        else if (template.CompareTag("PipeBRTemplate"))
            return PipeBottomRight;
        else if (template.CompareTag("PipeBLTemplate"))
            return PipeBottomLeft;
        else if (template.CompareTag("PipeTLTemplate"))
            return PipeTopLeft;
        else return null;
    }
}
