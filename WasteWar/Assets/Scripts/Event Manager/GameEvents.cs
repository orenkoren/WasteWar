using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{   
    public static event EventHandler<TemplateData> TemplateSelectedListeners;
    public static event EventHandler<TemplateData> LeftClickPressedListeners;
    public static event EventHandler<RaycastHit> RightClickPressedListeners;
    public static event EventHandler<Vector3> MouseOverListeners;
    public static event EventHandler<int> NodeUsedUpListeners;
    public static event EventHandler<ResourceGrid> LoadingTerrainTexturesListeners;
    public static event EventHandler<int> BuildingRotationListeners;
   
    public static void FireTemplateSelected(object sender, TemplateData data) => 
                                            TemplateSelectedListeners?.Invoke(sender, data);
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
    public static void FireBuildingRotation(object sender, int kekw) =>
                                            BuildingRotationListeners?.Invoke(sender,kekw);
}

public class TemplateData
{
    public GameObject TemplateStructure { get; set; }
    public Vector3 mousePos { get;  set; }

    public TemplateData()
    {
        TemplateStructure = null;
    }
}
