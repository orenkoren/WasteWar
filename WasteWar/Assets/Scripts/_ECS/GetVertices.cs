using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GetVertices : MonoBehaviour
{
    [HideInInspector]
    public List<float3> vertices;
    void Start()
    {
        var filters = GetComponentsInChildren<MeshFilter>();
        foreach (var filter in filters)
        {
            var verts = filter.mesh.vertices;
            for (int i = 0; i < verts.Length; i++)
            {
                vertices.Add(transform.TransformPoint(verts[i]));
            }
        }
    }


}
