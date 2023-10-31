using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class SingleBeltAuthoring : MonoBehaviour
{
    public float3 Direction;

    private void OnValidate()
    {
        //set the direction based on the object rotation
        Direction = transform.rotation * math.forward();
    }

    public class SingleBeltBaker : Baker<SingleBeltAuthoring>
    {
        public override void Bake(SingleBeltAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SingleBelt()
            {
                ConveyDirection = authoring.Direction,
            });
        }
    }
}