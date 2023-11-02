using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class BeltChildAuthoring : MonoBehaviour
{
    public class BeltChildBaker : Baker<BeltChildAuthoring>
    {
        public override void Bake(BeltChildAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BeltChild());
        }
    }
}

public struct BeltChild : IComponentData { }