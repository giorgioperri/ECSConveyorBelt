using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class PlayerControllerAuthoring : MonoBehaviour
{
    public float speed;

    public class PlayerControllerBaker : Baker<PlayerControllerAuthoring>
    {
        public override void Bake(PlayerControllerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerController
            {
                speed = authoring.speed
            });
        }
    }
}
