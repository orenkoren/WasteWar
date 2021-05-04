using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class DisabledComponentConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity e, HybridEntitySync syncComp) =>
        {
            Debug.Log("adding syncComp for" + e.Index.ToString());
            AddHybridComponent(syncComp);
        });
    }
}
