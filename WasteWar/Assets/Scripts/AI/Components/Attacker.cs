﻿using UnityEngine;
using UnityEngine.AI;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct Attacker : IComponentData
{
    public Entity entity;
    public Entity playerBase;
    public float speed;
}