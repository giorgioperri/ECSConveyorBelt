using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct SingleBelt : IComponentData
{
    public float3 ConveyDirection;
}