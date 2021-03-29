using System;
using UnityEngine;

public static class GameEvents
{   
    public static event EventHandler<TemplateData> TemplateSelectedListeners;
    public static event EventHandler<StructureType> TemplateSelectedTypeListeners;
    public static event EventHandler<Vector3> StructurePlacedListeners;
    public static void FireTemplateSelected(object sender, TemplateData data) => 
                                            TemplateSelectedListeners?.Invoke(sender, data);
    public static void FireTemplateSelectedType(object sender, StructureType type) =>
                                        TemplateSelectedTypeListeners?.Invoke(sender, type);
    public static void FireStructurePlaced(object sender, Vector3 mousePos) =>
                                            StructurePlacedListeners?.Invoke(sender, mousePos);
}

public class TemplateData
{
    public GameObject TemplateStructure { get; set; }
    public Vector3 mousePos { get;  set; }
    public bool IsSelected { get;  set; }
    public StructureType StructureType { get; set; }
}
