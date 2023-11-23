using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class EndTargetAuthoring : MonoBehaviour
{
    public int id;
    public class EndTargetBaker : Baker<EndTargetAuthoring>
    {
        public override void Bake(EndTargetAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EndTarget()
            {
                id = authoring.id
            });
        }
    }
}