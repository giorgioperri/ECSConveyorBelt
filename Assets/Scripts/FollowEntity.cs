using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class FollowEntity : MonoBehaviour
{
    [SerializeField] private Entity _entityToFollow;
    private EntityManager _manager;
    private NativeArray<Entity> _entities;

    void Start()
    {
        _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        var finder = _manager.CreateEntityQuery(typeof(PlayerController), typeof(SceneSection));
        _entities = finder.ToEntityArray(Allocator.TempJob);

        
    }
    
    void LateUpdate()
    {
        
        LocalTransform Ltransform = _manager.GetComponentData<LocalTransform>(_entities[0]);

        transform.position = Ltransform.Position;
    }
}
