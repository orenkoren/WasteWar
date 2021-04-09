using System;
using UnityEngine;

public static class GameEvents
{   
    public static event EventHandler<TemplateData> TemplateSelectedListeners;
    public static event EventHandler<TemplateData> StructurePlacedListeners;
    public static event EventHandler<Vector3> MouseOverListeners;
    public static void FireTemplateSelected(object sender, TemplateData data) => 
                                            TemplateSelectedListeners?.Invoke(sender, data);
    public static void FireStructurePlaced(object sender, TemplateData data ) =>
                                            StructurePlacedListeners?.Invoke(sender, data);
    public static void FireMouseOver(object sender, Vector3 hitPoint) =>
                                            MouseOverListeners?.Invoke(sender, hitPoint);
}

public class TemplateData
{
    public GameObject TemplateStructure { get; set; }
    public Vector3 mousePos { get;  set; }
    public StructureType StructureType { get; set; }

    public TemplateData()
    {
        StructureType = StructureType.NONE;
    }
}
