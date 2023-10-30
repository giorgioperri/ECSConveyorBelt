using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Transforms;



public partial struct SpawnerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var Spawn = new Spawn
        {
            chance = UnityEngine.Random.Range(1, 5),
            ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged),
            DeltaTime = SystemAPI.Time.DeltaTime
        };

        Spawn.Schedule();
    }

    public partial struct Spawn : IJobEntity
    {
        public EntityCommandBuffer ECB;
        public float DeltaTime;
        public int chance;

        void Execute(Entity entity, ref Spawner spawner)
        {
            spawner.timer += DeltaTime;

            if (spawner.timer > spawner.maxTImer)
            {
                if (chance == 1)
                {
                    var newobject = ECB.Instantiate(spawner.objectToSpawn);
                    var transform = LocalTransform.FromPositionRotation(spawner.position, spawner.rotation);
                    ECB.SetComponent(newobject, transform);

                }
                else
                {
                    var newobject = ECB.Instantiate(spawner.objectToSpawn);
                    var transform = LocalTransform.FromPositionRotation(spawner.position, spawner.rotation);
                    ECB.SetComponent(newobject, transform);
                }

                spawner.timer = 0;
            }
        }
    }
}

