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
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Random = Unity.Mathematics.Random;

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
        state.Dependency = new CollisionEventBallsJob
        {
            PhysicsVelocityData = SystemAPI.GetComponentLookup<PhysicsVelocity>(),
            ConveyableObjectData = SystemAPI.GetComponentLookup<ConveyableObject>(),
            LocalTransformData = SystemAPI.GetComponentLookup<LocalTransform>(),
            ElapsedTime = SystemAPI.Time.ElapsedTime,
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }

    [BurstCompile]
    struct CollisionEventBallsJob : ICollisionEventsJob
    {
        public ComponentLookup<PhysicsVelocity> PhysicsVelocityData;
        public ComponentLookup<ConveyableObject> ConveyableObjectData;
        public ComponentLookup<LocalTransform> LocalTransformData;
        public double ElapsedTime;

        public void Execute(CollisionEvent collisionEvent)
        {
            Entity entityA = collisionEvent.EntityA;
            Entity entityB = collisionEvent.EntityB;

            bool isBodyAConveyable = ConveyableObjectData.HasComponent(entityA);
            bool isBodyBConveyable = ConveyableObjectData.HasComponent(entityB);

            if (isBodyAConveyable && isBodyBConveyable)
            {
                var velocityComponentA = PhysicsVelocityData[entityA];
                var velocityComponentB = PhysicsVelocityData[entityB];
                
                //lift the ball up a bit so it doesn't collide again
                var transformComponentA = LocalTransformData[entityA];
                var transformComponentB = LocalTransformData[entityB];
                
                transformComponentA.Position.y += 0.1f;
                transformComponentB.Position.y += 0.1f;
                
                LocalTransformData[entityA] = transformComponentA;
                LocalTransformData[entityB] = transformComponentB;
                
                //set the linear velocity of the ball randomly, but with a fixed y of 3
                //the velocity of the 2 balls should be the different
                
                var random = new Random((uint) (ElapsedTime * 1000));
                var randomX = random.NextFloat(-1f, 1f);
                var randomZ = random.NextFloat(-1f, 1f);
                
                velocityComponentA.Linear = new float3(randomX, 3f, randomZ);
                velocityComponentB.Linear = new float3(-randomX, 3f, -randomZ);

                PhysicsVelocityData[entityA] = velocityComponentA;
                PhysicsVelocityData[entityB] = velocityComponentB;
            }
        }
    }
}