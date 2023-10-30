using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class SpawnerAuthoring : MonoBehaviour
{
    public GameObject toSpawn1;
    public GameObject toSpawn2;
    public GameObject toSpawn3;
    public GameObject toSpawn4;
    public GameObject toSpawn5;

    public float timer;
    public float maxTimer;


    public float3 position;
    public Quaternion rotation;
}

public class SpawnerBaker : Baker<SpawnerAuthoring>
{
    public override void Bake(SpawnerAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new Spawner
        {
            objectToSpawn = GetEntity(authoring.toSpawn1, TransformUsageFlags.Dynamic),
            objectToSpawn2 = GetEntity(authoring.toSpawn2, TransformUsageFlags.Dynamic),
            objectToSpawn3 = GetEntity(authoring.toSpawn3, TransformUsageFlags.Dynamic),
            objectToSpawn4 = GetEntity(authoring.toSpawn4, TransformUsageFlags.Dynamic),
            objectToSpawn5 = GetEntity(authoring.toSpawn5, TransformUsageFlags.Dynamic),
            timer = authoring.timer,
            maxTImer = authoring.maxTimer,
            position = authoring.position,
            rotation = authoring.rotation
        });
    }
}
