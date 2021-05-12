using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class CounterConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Counter counter) =>
        {
            AddHybridComponent(counter);
        });
    }
}
