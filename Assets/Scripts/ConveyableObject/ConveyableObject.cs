using Unity.Entities;
using UnityEngine;

public struct ConveyableObject : IComponentData 
{
    public int id;
    public int repulsionForce;
}