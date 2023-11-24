using Unity.Assertions;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Unity.Physics.Extensions
{
    // Attaches a virtual spring to the picked entity
    [UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
    public partial class MousePickSystem : SystemBase
    {
        public const float k_MaxDistance = 100.0f;
        public NativeReference<RotationData> RotationDataRef;
        public JobHandle? PickJobHandle;

        public struct RotationData
        { }

        public MousePickSystem()
        {
            RotationDataRef =
                new NativeReference<RotationData>(Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            RotationDataRef.Value = new RotationData();
        }

        protected override void OnCreate()
        {
            RequireForUpdate<MousePick>();
        }

        protected override void OnDestroy()
        {
            RotationDataRef.Dispose();
        }

        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonDown(0) && (Camera.main != null))
            {
                UnityEngine.Ray unityRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

                var world = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

                // Schedule picking job, after the collision world has been built
                Dependency = new Pick
                {
                    CollisionWorld = world.CollisionWorld,
                    RotationDataRef = RotationDataRef,
                    RayInput = new RaycastInput
                    {
                        Start = unityRay.origin,
                        End = unityRay.origin + unityRay.direction * k_MaxDistance,
                        Filter = CollisionFilter.Default,
                    },
                    Near = Camera.main.nearClipPlane,
                    Forward = Camera.main.transform.forward,
                    IgnoreTriggers = SystemAPI.GetSingleton<MousePick>().IgnoreTriggers,
                    LocalTransformData = SystemAPI.GetComponentLookup<LocalTransform>()
                }.Schedule(Dependency);

                PickJobHandle = Dependency;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (PickJobHandle != null)
                {
                    PickJobHandle.Value.Complete();
                }

                RotationDataRef.Value = new RotationData();
            }
        }

        [BurstCompile]
        struct Pick : IJob
        {
            [ReadOnly] public CollisionWorld CollisionWorld;
            public NativeReference<RotationData> RotationDataRef;
            public RaycastInput RayInput;
            public float Near;
            public float3 Forward;
            [ReadOnly] public bool IgnoreTriggers;

            public ComponentLookup<LocalTransform> LocalTransformData;

            public void Execute()
            {
                var mousePickCollector =
                    new MousePickCollector(1.0f, CollisionWorld.Bodies, CollisionWorld.NumDynamicBodies);
                mousePickCollector.IgnoreTriggers = IgnoreTriggers;

                if (CollisionWorld.CastRay(RayInput, ref mousePickCollector))
                {
                    RigidBody hitBody = CollisionWorld.Bodies[mousePickCollector.Hit.RigidBodyIndex];

                    Debug.Log(hitBody.Entity.Index);
                    
                    LocalTransformData[hitBody.Entity] = new LocalTransform
                    {
                        Position = LocalTransformData[hitBody.Entity].Position,
                        Rotation = math.mul(LocalTransformData[hitBody.Entity].Rotation, quaternion.RotateY(math.radians(90))),
                        Scale = LocalTransformData[hitBody.Entity].Scale
                    };

                }
            }
        }
    }

    // A mouse pick collector which stores every hit. Based off the ClosestHitCollector
    [BurstCompile]
    public struct MousePickCollector : ICollector<RaycastHit>
    {
        public bool IgnoreTriggers;
        public bool IgnoreStatic;
        public NativeArray<RigidBody> Bodies;
        public int NumDynamicBodies;

        public bool EarlyOutOnFirstHit => false;
        public float MaxFraction { get; private set; }
        public int NumHits { get; private set; }

        private RaycastHit m_ClosestHit;
        public RaycastHit Hit => m_ClosestHit;

        public MousePickCollector(float maxFraction, NativeArray<RigidBody> rigidBodies, int numDynamicBodies)
        {
            m_ClosestHit = default(RaycastHit);
            MaxFraction = maxFraction;
            NumHits = 0;
            IgnoreTriggers = true;
            IgnoreStatic = true;
            Bodies = rigidBodies;
            NumDynamicBodies = numDynamicBodies;
        }

        #region ICollector

        public bool AddHit(RaycastHit hit)
        {
            Assert.IsTrue(hit.Fraction <= MaxFraction);

            var isAcceptable = true;
            if (IgnoreStatic)
            {
                isAcceptable = isAcceptable && (hit.RigidBodyIndex >= 0) && (hit.RigidBodyIndex < NumDynamicBodies);
            }

            if (IgnoreTriggers)
            {
                isAcceptable = isAcceptable &&
                    hit.Material.CollisionResponse != CollisionResponsePolicy.RaiseTriggerEvents;
            }

            if (!isAcceptable)
            {
                return false;
            }

            MaxFraction = hit.Fraction;
            m_ClosestHit = hit;
            NumHits = 1;
            return true;
        }

        #endregion
    }
}
