using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class ConveyableObjectAuthoring : MonoBehaviour
{
    public int id;
    public int repulsionForce;
    public class ConveyableObjectBaker : Baker<ConveyableObjectAuthoring>
    {
        public override void Bake(ConveyableObjectAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ConveyableObject()
            {
                id = authoring.id,
                repulsionForce = authoring.repulsionForce,
            });
        }
    }
}