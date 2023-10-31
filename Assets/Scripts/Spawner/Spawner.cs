using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
public struct Spawner : IComponentData
{
    public float timer;
    public float maxTImer;
    public Vector3 position;
    public Quaternion rotation;
    public Entity objectToSpawn;
    public Entity objectToSpawn2;
    public Entity objectToSpawn3;
    public Entity objectToSpawn4;
    public Entity objectToSpawn5;
}