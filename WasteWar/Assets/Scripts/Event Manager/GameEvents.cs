using System;
using Unity.Entities;
using UnityEngine;

public static class GameEvents
{
    public static event EventHandler<TemplateData> TemplateSelectedListeners;
    public static event EventHandler<TemplateData> StructurePlacedListeners;
    public static void FireTemplateSelected(object sender, TemplateData data) => 
                                            TemplateSelectedListeners?.Invoke(sender, data);
    public static void FireStructurePlaced(object sender, TemplateData data ) =>
                                            StructurePlacedListeners?.Invoke(sender, data);
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
