using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct GridComponent : IComponentData
{
    public int Width;
    public int Height;
    public int CellSize;
    public bool ShouldVisualize;
}
