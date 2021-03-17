using System;
using UnityEngine;

public static class GameEvents
{
    public static event EventHandler<DrawData> DrawStateChangedListeners;
    public static void FireDrawStateChanged(object sender, DrawData structurePrefab) => 
                                            DrawStateChangedListeners?.Invoke(sender, structurePrefab);
}

public class DrawData
{
    public GameObject StructurePrefab { get; set; }
    public Vector3 StructureLocation { get; set; }

}
