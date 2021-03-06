using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class DisabledComponentConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((HybridDestructionSync syncComp) =>
        {
            AddHybridComponent(syncComp);
        });
    }
}
