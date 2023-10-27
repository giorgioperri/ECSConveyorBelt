using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;



public partial struct PlayerControllerSystem : ISystem
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

        var Move = new Move
        {
            ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged),

            forward = Input.GetKey(KeyCode.W) ? 1 : 0,
            backward = Input.GetKey(KeyCode.S) ? 1 : 0,
            left = Input.GetKey(KeyCode.A) ? 1 : 0,
            right = Input.GetKey(KeyCode.D) ? 1 : 0,
            jump = Input.GetKey(KeyCode.Space) ? 1 : 0,

            DeltaTime = SystemAPI.Time.DeltaTime,
        };

        Move.Schedule();
    }

    public partial struct Move : IJobEntity
    {
        public EntityCommandBuffer ECB;
        public float DeltaTime;

        public int forward;
        public int backward;
        public int left;
        public int right;
        public int jump;

        void Execute(Entity entity, ref PlayerController playerController, ref LocalTransform localTransform)
        {
            Debug.Log("Uora ma minu ueue");
            localTransform.Position += new float3((-left + right) * playerController.speed * DeltaTime, jump * playerController.speed * DeltaTime, (-backward + forward) * playerController.speed * DeltaTime);

            //transportation.position.x = DeltaTime * transportation.speed;
            //localTransform.Position.x += transportation.position.x;
        }
    }
}

