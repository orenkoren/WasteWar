using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    public static event EventHandler<TemplateData> TemplateSelectedListeners;
    public static event EventHandler<bool> BuildingRotationListeners;
    public static event EventHandler<GameObject> PipePlacedListeners;
    public static event EventHandler<GameObject> PipeDeletedListeners;
    public static event EventHandler<GameObject> BuildingPlacedListeners;
    //used to memorize previous pipe rotation when spawning next pipe
    public static event EventHandler<string> PipePlaced2Listeners;

    public static event EventHandler<TemplateData> LeftClickPressedListeners;
    public static event EventHandler<int> LeftClickUpListeners;
    public static event EventHandler<TemplateData> ContinuousLeftClickPressListeners;
    public static event EventHandler<RaycastHit> RightClickPressedListeners;
    public static event EventHandler<int> MiddleMouseClickPressedListeners;
    public static event EventHandler<Vector3> MouseOverListeners;
    public static event EventHandler<Vector3> MouseMovListeners;

    public static event EventHandler<int> NodeUsedUpListeners;
    public static event EventHandler<int> EraseResourceGameObjectListeners;
    public static event EventHandler<Dictionary<int, Resource>> ResourcesGeneratedListeners;

    public static void FireTemplateSelected(object sender, TemplateData data) =>
                                            TemplateSelectedListeners?.Invoke(sender, data);
    public static void FireBuildingRotation(object sender, bool isCurvedModeOn) =>
                                        BuildingRotationListeners?.Invoke(sender, isCurvedModeOn);
    public static void FirePipePlaced(object sender, GameObject structure) =>
                                      PipePlacedListeners?.Invoke(sender, structure);
    public static void FirePipeDeleted(object sender, GameObject structure) =>
                                  PipeDeletedListeners?.Invoke(sender, structure);
    public static void FireBuildingPlaced(object sender, GameObject structure) =>
                                      BuildingPlacedListeners?.Invoke(sender, structure);
    public static void FirePipePlaced2(object sender, string prefabName) =>
                                  PipePlaced2Listeners?.Invoke(sender, prefabName);
    public static void FireLeftClickPressed(object sender, TemplateData data) =>
                                            LeftClickPressedListeners?.Invoke(sender, data);
    public static void FireLeftClickUp(object sender, int i) =>
                                            LeftClickUpListeners?.Invoke(sender, i);
    public static void FireRightClickPressed(object sender, RaycastHit data) =>
                                            RightClickPressedListeners?.Invoke(sender, data);
    public static void FireContinuousLeftClickPress(object sender, TemplateData data) =>
                                             ContinuousLeftClickPressListeners?.Invoke(sender, data);
    public static void FireMiddleMouseClickPressed(object sender, int data) =>
                                             MiddleMouseClickPressedListeners?.Invoke(sender, data);
    public static void FireMouseOver(object sender, Vector3 hitPoint) =>
                                            MouseOverListeners?.Invoke(sender, hitPoint);
    public static void FireNodeUsedUp(object sender, int locationKey) =>
                                            NodeUsedUpListeners?.Invoke(sender, locationKey);
    public static void FireEraseResourceGameObject(object sender, int locationKey) =>
                                            EraseResourceGameObjectListeners?.Invoke(sender, locationKey);
    public static void FireResourcesGenerated(object sender, Dictionary<int, Resource> resources) =>
                                            ResourcesGeneratedListeners?.Invoke(sender, resources);
    public static void FireMouseMov(object sender, Vector3 data) =>
                                            MouseMovListeners?.Invoke(sender, data);

}

public class TemplateData
{
    public GameObject TemplateStructure { get; set; }
    public Vector3 MousePos { get; set; }

    public TemplateData()
    {
        TemplateStructure = null;
    }
}
