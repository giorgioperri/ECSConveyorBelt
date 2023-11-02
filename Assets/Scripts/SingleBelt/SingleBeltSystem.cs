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
public partial struct SingleBeltSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SingleBelt>();
        state.RequireForUpdate<SimulationSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency = new CollisionEventConveyJob
        {
            SingleBeltData = SystemAPI.GetComponentLookup<SingleBelt>(),
            PhysicsVelocityData = SystemAPI.GetComponentLookup<PhysicsVelocity>(),
            BeltChildData = SystemAPI.GetComponentLookup<BeltChild>(),
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }

    [BurstCompile]
    struct CollisionEventConveyJob : ICollisionEventsJob
    {
        [ReadOnly] public ComponentLookup<SingleBelt> SingleBeltData;
        public ComponentLookup<BeltChild> BeltChildData;
        public ComponentLookup<PhysicsVelocity> PhysicsVelocityData;

        public void Execute(CollisionEvent collisionEvent)
        {
            Entity entityA = collisionEvent.EntityA;
            Entity entityB = collisionEvent.EntityB;

            bool isBodyADynamic = PhysicsVelocityData.HasComponent(entityA);
            bool isBodyBDynamic = PhysicsVelocityData.HasComponent(entityB);

            bool isBodyAConveyor = SingleBeltData.HasComponent(entityA);
            bool isBodyBConveyor = SingleBeltData.HasComponent(entityB);

            bool isBodyAChild = BeltChildData.HasComponent(entityA);
            bool isBodyBChild = BeltChildData.HasComponent(entityB);

            if (isBodyAConveyor && isBodyBDynamic && !isBodyAChild)
            {
                var impulseComponent = SingleBeltData[entityA];
                var velocityComponent = PhysicsVelocityData[entityB];
                velocityComponent.Linear = impulseComponent.ConveyDirection;
                PhysicsVelocityData[entityB] = velocityComponent;
            }

            if (isBodyBConveyor && isBodyADynamic && !isBodyBChild)
            {
                var impulseComponent = SingleBeltData[entityB];
                var velocityComponent = PhysicsVelocityData[entityA];
                velocityComponent.Linear = impulseComponent.ConveyDirection;
                PhysicsVelocityData[entityA] = velocityComponent;
            }
        }
    }
}