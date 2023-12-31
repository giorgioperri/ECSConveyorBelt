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
            drop = Input.GetKey(KeyCode.LeftControl) ? 1 : 0,
            rotateX = Input.GetAxis("Mouse X"),
            rotateY = Input.GetAxis("Mouse Y"),
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
        public int drop;
        public float rotateX;
        public float rotateY;

        void Execute(Entity entity, ref PlayerController playerController, ref LocalTransform localTransform)
        {
            float3 requestedMovementDirection = float3.zero;

            float3 aheadMov = math.forward(localTransform.Rotation);
            float3 sideMov = math.cross(Vector3.up, aheadMov);
            float3 flight = new float3(0, jump - drop, 0);

            float3 localMov = aheadMov * (-backward + forward) + sideMov * (-left + right) + flight;

            localTransform.Position = localTransform.Position + localMov * playerController.speed;
            localTransform.Rotation.value.y = CameraTag.Instance.Rotation.y;
        }
    }
}

