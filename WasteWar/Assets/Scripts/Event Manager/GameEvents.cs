using System;
using UnityEngine;

public static class GameEvents
{   
    public static event EventHandler<TemplateData> TemplateSelectedListeners;
    public static event EventHandler<int> BuildingRotationListeners;
    public static event EventHandler<GameObject> PipePlacedListeners;
    //used to memorize previous pipe rotation when spawning next pipe
    public static event EventHandler<string> PipePlaced2Listeners;

    public static event EventHandler<TemplateData> LeftClickPressedListeners;
    public static event EventHandler<RaycastHit> RightClickPressedListeners;
    public static event EventHandler<Vector3> MouseOverListeners;

    public static event EventHandler<int> NodeUsedUpListeners;
    public static event EventHandler<ResourceGrid> LoadingTerrainTexturesListeners;
   
    public static void FireTemplateSelected(object sender, TemplateData data) => 
                                            TemplateSelectedListeners?.Invoke(sender, data);
    public static void FireBuildingRotation(object sender, int i) =>
                                        BuildingRotationListeners?.Invoke(sender, i);
    public static void FirePipePlaced(object sender, GameObject structure) =>
                                      PipePlacedListeners?.Invoke(sender, structure);
    public static void FirePipe2Placed(object sender, string prefabName) =>
                                  PipePlaced2Listeners?.Invoke(sender, prefabName);
    public static void FireLeftClickPressed(object sender, TemplateData data ) =>
                                            LeftClickPressedListeners?.Invoke(sender, data);
    public static void FireRightClickPressed(object sender, RaycastHit data) =>
                                            RightClickPressedListeners?.Invoke(sender, data);
    public static void FireMouseOver(object sender, Vector3 hitPoint) =>
                                            MouseOverListeners?.Invoke(sender, hitPoint);
    public static void FireNodeUsedUp(object sender, int locationKey) =>
                                            NodeUsedUpListeners?.Invoke(sender, locationKey);
    public static void FireLoadingTerrainTextures(object sender, ResourceGrid resources) =>
                                            LoadingTerrainTexturesListeners?.Invoke(sender, resources);
}

public class TemplateData
{
    public GameObject TemplateStructure { get; set; }
    public Vector3 MousePos { get;  set; }

    public TemplateData()
    {
        TemplateStructure = null;
    }
}
