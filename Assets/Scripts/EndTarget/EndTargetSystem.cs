using System.Collections;
using System.Collections.Generic;
using Events;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct EndTargetSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndTarget>();
        state.RequireForUpdate<SimulationSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        state.Dependency = new CollisionEventTargetJob
        {
            EndTargetData = SystemAPI.GetComponentLookup<EndTarget>(),
            PhysicsVelocityData = SystemAPI.GetComponentLookup<PhysicsVelocity>(),
            ConveyableData = SystemAPI.GetComponentLookup<ConveyableObject>(),
            ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged)

        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }

    [BurstCompile]
    struct CollisionEventTargetJob : ICollisionEventsJob
    {
        [ReadOnly] public ComponentLookup<EndTarget> EndTargetData;
        public ComponentLookup<PhysicsVelocity> PhysicsVelocityData;
        public ComponentLookup<ConveyableObject> ConveyableData;
        public EntityCommandBuffer ECB;

        public void Execute(CollisionEvent collisionEvent)
        {
            Entity entityA = collisionEvent.EntityA;
            Entity entityB = collisionEvent.EntityB;

            bool isBodyADynamic = PhysicsVelocityData.HasComponent(entityA);
            bool isBodyBDynamic = PhysicsVelocityData.HasComponent(entityB);

            bool isBodyAEndTarget = EndTargetData.HasComponent(entityA);
            bool isBodyBEndTarget = EndTargetData.HasComponent(entityB);

            bool isBodyAConveyable = ConveyableData.HasComponent(entityA);
            bool isBodyBConveyable = ConveyableData.HasComponent(entityB);


            if (isBodyAEndTarget && isBodyBDynamic && isBodyBConveyable)
            {
                int idEnd = EndTargetData[entityA].id;
                int idConveyable = ConveyableData[entityB].id;

                if(idEnd == idConveyable)
                {
                    Debug.Log("Good Destruction");
                }
                else
                {
                    Debug.Log("Bad Destruction");
                }

                ECB.DestroyEntity(entityB);
            }

            if (isBodyBEndTarget && isBodyADynamic && isBodyAConveyable)
            {
                int idEnd = EndTargetData[entityB].id;
                int idConveyable = ConveyableData[entityA].id;

                if (idEnd == idConveyable)
                {
                    Debug.Log("Good Destruction");
                }
                else
                {
                    Debug.Log("Bad Destruction");
                }

                ECB.DestroyEntity(entityA);
            }
        }
    }
}